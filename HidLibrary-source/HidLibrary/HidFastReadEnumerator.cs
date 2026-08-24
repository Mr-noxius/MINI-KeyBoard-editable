using System;
using System.Collections.Generic;
using System.Linq;

namespace HidLibrary;

public class HidFastReadEnumerator : IHidEnumerator
{
	public bool IsConnected(string devicePath)
	{
		return HidDevices.IsConnected(devicePath);
	}

	public IHidDevice GetDevice(string devicePath)
	{
		return Enumerate(devicePath).FirstOrDefault();
	}

	public IEnumerable<IHidDevice> Enumerate()
	{
		return HidDevices.EnumerateDevices().Select((Func<HidDevices.DeviceInfo, IHidDevice>)((HidDevices.DeviceInfo d) => new HidFastReadDevice(d.Path, d.Description)));
	}

	public IEnumerable<IHidDevice> Enumerate(string devicePath)
	{
		return (from x in HidDevices.EnumerateDevices()
			where x.Path == devicePath
			select x).Select((Func<HidDevices.DeviceInfo, IHidDevice>)((HidDevices.DeviceInfo d) => new HidFastReadDevice(d.Path, d.Description)));
	}

	public IEnumerable<IHidDevice> Enumerate(int vendorId, params int[] productIds)
	{
		return (from d in HidDevices.EnumerateDevices()
			select new HidFastReadDevice(d.Path, d.Description) into f
			where f.Attributes.VendorId == vendorId && productIds.Contains(f.Attributes.ProductId)
			select f).Select((Func<HidFastReadDevice, IHidDevice>)((HidFastReadDevice d) => d));
	}

	public IEnumerable<IHidDevice> Enumerate(int vendorId)
	{
		return (from d in HidDevices.EnumerateDevices()
			select new HidFastReadDevice(d.Path, d.Description) into f
			where f.Attributes.VendorId == vendorId
			select f).Select((Func<HidFastReadDevice, IHidDevice>)((HidFastReadDevice d) => d));
	}
}
