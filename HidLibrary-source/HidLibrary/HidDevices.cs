using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace HidLibrary;

public class HidDevices
{
	internal class DeviceInfo
	{
		public string Path { get; set; }

		public string Description { get; set; }
	}

	private static Guid _hidClassGuid = Guid.Empty;

	private static Guid HidClassGuid
	{
		get
		{
			if (_hidClassGuid.Equals(Guid.Empty))
			{
				NativeMethods.HidD_GetHidGuid(ref _hidClassGuid);
			}
			return _hidClassGuid;
		}
	}

	public static bool IsConnected(string devicePath)
	{
		return EnumerateDevices().Any((DeviceInfo x) => x.Path == devicePath);
	}

	public static HidDevice GetDevice(string devicePath)
	{
		return Enumerate(devicePath).FirstOrDefault();
	}

	public static IEnumerable<HidDevice> Enumerate()
	{
		return from x in EnumerateDevices()
			select new HidDevice(x.Path, x.Description);
	}

	public static IEnumerable<HidDevice> Enumerate(string devicePath)
	{
		return from x in EnumerateDevices()
			where x.Path == devicePath
			select new HidDevice(x.Path, x.Description);
	}

	public static IEnumerable<HidDevice> Enumerate(int vendorId, params int[] productIds)
	{
		return from x in EnumerateDevices()
			select new HidDevice(x.Path, x.Description) into x
			where x.Attributes.VendorId == vendorId && productIds.Contains(x.Attributes.ProductId)
			select x;
	}

	public static IEnumerable<HidDevice> Enumerate(int vendorId, int productId, ushort UsagePage)
	{
		return from x in EnumerateDevices()
			select new HidDevice(x.Path, x.Description) into x
			where x.Attributes.VendorId == vendorId && productId == (ushort)x.Attributes.ProductId && (ushort)x.Capabilities.UsagePage == UsagePage
			select x;
	}

	public static IEnumerable<HidDevice> Enumerate(int vendorId)
	{
		return from x in EnumerateDevices()
			select new HidDevice(x.Path, x.Description) into x
			where x.Attributes.VendorId == vendorId
			select x;
	}

	internal static IEnumerable<DeviceInfo> EnumerateDevices()
	{
		List<DeviceInfo> list = new List<DeviceInfo>();
		Guid classGuid = HidClassGuid;
		IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref classGuid, null, 0, 18);
		if (deviceInfoSet.ToInt64() != -1)
		{
			NativeMethods.SP_DEVINFO_DATA deviceInfoData = CreateDeviceInfoData();
			int num = 0;
			while (NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, num, ref deviceInfoData))
			{
				num++;
				NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = default(NativeMethods.SP_DEVICE_INTERFACE_DATA);
				deviceInterfaceData.cbSize = Marshal.SizeOf((object)deviceInterfaceData);
				int num2 = 0;
				while (NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoSet, ref deviceInfoData, ref classGuid, num2, ref deviceInterfaceData))
				{
					num2++;
					string devicePath = GetDevicePath(deviceInfoSet, deviceInterfaceData);
					string description = GetBusReportedDeviceDescription(deviceInfoSet, ref deviceInfoData) ?? GetDeviceDescription(deviceInfoSet, ref deviceInfoData);
					list.Add(new DeviceInfo
					{
						Path = devicePath,
						Description = description
					});
				}
			}
			NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
		}
		return list;
	}

	private static NativeMethods.SP_DEVINFO_DATA CreateDeviceInfoData()
	{
		NativeMethods.SP_DEVINFO_DATA sP_DEVINFO_DATA = default(NativeMethods.SP_DEVINFO_DATA);
		sP_DEVINFO_DATA.cbSize = Marshal.SizeOf((object)sP_DEVINFO_DATA);
		sP_DEVINFO_DATA.DevInst = 0;
		sP_DEVINFO_DATA.ClassGuid = Guid.Empty;
		sP_DEVINFO_DATA.Reserved = IntPtr.Zero;
		return sP_DEVINFO_DATA;
	}

	private static string GetDevicePath(IntPtr deviceInfoSet, NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
	{
		int requiredSize = 0;
		NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA deviceInterfaceDetailData = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			Size = ((IntPtr.Size == 4) ? (4 + Marshal.SystemDefaultCharSize) : 8)
		};
		NativeMethods.SetupDiGetDeviceInterfaceDetailBuffer(deviceInfoSet, ref deviceInterfaceData, IntPtr.Zero, 0, ref requiredSize, IntPtr.Zero);
		return NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoSet, ref deviceInterfaceData, ref deviceInterfaceDetailData, requiredSize, ref requiredSize, IntPtr.Zero) ? deviceInterfaceDetailData.DevicePath : null;
	}

	private static string GetDeviceDescription(IntPtr deviceInfoSet, ref NativeMethods.SP_DEVINFO_DATA devinfoData)
	{
		byte[] array = new byte[1024];
		int requiredSize = 0;
		int propertyRegDataType = 0;
		NativeMethods.SetupDiGetDeviceRegistryProperty(deviceInfoSet, ref devinfoData, 0, ref propertyRegDataType, array, array.Length, ref requiredSize);
		return array.ToUTF8String();
	}

	private static string GetBusReportedDeviceDescription(IntPtr deviceInfoSet, ref NativeMethods.SP_DEVINFO_DATA devinfoData)
	{
		byte[] array = new byte[1024];
		if (Environment.OSVersion.Version.Major > 5)
		{
			ulong propertyDataType = 0uL;
			int requiredSize = 0;
			if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref devinfoData, ref NativeMethods.DEVPKEY_Device_BusReportedDeviceDesc, ref propertyDataType, array, array.Length, ref requiredSize, 0u))
			{
				return array.ToUTF16String();
			}
		}
		return null;
	}
}
