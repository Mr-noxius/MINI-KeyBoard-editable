using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace HidLibrary;

public class HidDevice : IHidDevice, IDisposable
{
	protected delegate HidDeviceData ReadDelegate(int timeout);

	protected delegate HidReport ReadReportDelegate(int timeout);

	private delegate bool WriteDelegate(byte[] data, int timeout);

	private delegate bool WriteReportDelegate(HidReport report, int timeout);

	private readonly string _description;

	private readonly string _devicePath;

	private readonly HidDeviceAttributes _deviceAttributes;

	private readonly HidDeviceCapabilities _deviceCapabilities;

	private DeviceMode _deviceReadMode = DeviceMode.NonOverlapped;

	private DeviceMode _deviceWriteMode = DeviceMode.NonOverlapped;

	private ShareMode _deviceShareMode = ShareMode.ShareRead | ShareMode.ShareWrite;

	private readonly HidDeviceEventMonitor _deviceEventMonitor;

	private bool _monitorDeviceEvents;

	public IntPtr Handle { get; private set; }

	public bool IsOpen { get; private set; }

	public bool IsConnected => HidDevices.IsConnected(_devicePath);

	public string Description => _description;

	public HidDeviceCapabilities Capabilities => _deviceCapabilities;

	public HidDeviceAttributes Attributes => _deviceAttributes;

	public string DevicePath => _devicePath;

	public bool MonitorDeviceEvents
	{
		get
		{
			return _monitorDeviceEvents;
		}
		set
		{
			if (value & !_monitorDeviceEvents)
			{
				_deviceEventMonitor.Init();
			}
			_monitorDeviceEvents = value;
		}
	}

	public event InsertedEventHandler Inserted;

	public event RemovedEventHandler Removed;

	internal HidDevice(string devicePath, string description = null)
	{
		_deviceEventMonitor = new HidDeviceEventMonitor(this);
		_deviceEventMonitor.Inserted += DeviceEventMonitorInserted;
		_deviceEventMonitor.Removed += DeviceEventMonitorRemoved;
		_devicePath = devicePath;
		_description = description;
		try
		{
			IntPtr intPtr = OpenDeviceIO(_devicePath, 0u);
			_deviceAttributes = GetDeviceAttributes(intPtr);
			_deviceCapabilities = GetDeviceCapabilities(intPtr);
			CloseDeviceIO(intPtr);
		}
		catch (Exception innerException)
		{
			throw new Exception($"Error querying HID device '{devicePath}'.", innerException);
		}
	}

	public override string ToString()
	{
		return $"VendorID={_deviceAttributes.VendorHexId}, ProductID={_deviceAttributes.ProductHexId}, Version={_deviceAttributes.Version}, DevicePath={_devicePath}";
	}

	public void OpenDevice()
	{
		OpenDevice(DeviceMode.NonOverlapped, DeviceMode.NonOverlapped, ShareMode.ShareRead | ShareMode.ShareWrite);
	}

	public void OpenDevice(DeviceMode readMode, DeviceMode writeMode, ShareMode shareMode)
	{
		if (!IsOpen)
		{
			_deviceReadMode = readMode;
			_deviceWriteMode = writeMode;
			_deviceShareMode = shareMode;
			try
			{
				Handle = OpenDeviceIO(_devicePath, readMode, 3221225472u, shareMode);
			}
			catch (Exception innerException)
			{
				IsOpen = false;
				throw new Exception("Error opening HID device.", innerException);
			}
			IsOpen = Handle.ToInt32() != -1;
		}
	}

	public void CloseDevice()
	{
		if (IsOpen)
		{
			CloseDeviceIO(Handle);
			IsOpen = false;
		}
	}

	public HidDeviceData Read()
	{
		return Read(0);
	}

	public HidDeviceData Read(int timeout)
	{
		if (IsConnected)
		{
			if (!IsOpen)
			{
				OpenDevice(_deviceReadMode, _deviceWriteMode, _deviceShareMode);
			}
			try
			{
				return ReadData(timeout);
			}
			catch
			{
				return new HidDeviceData(HidDeviceData.ReadStatus.ReadError);
			}
		}
		return new HidDeviceData(HidDeviceData.ReadStatus.NotConnected);
	}

	public void Read(ReadCallback callback)
	{
		Read(callback, 0);
	}

	public void Read(ReadCallback callback, int timeout)
	{
		ReadDelegate readDelegate = Read;
		HidAsyncState hidAsyncState = new HidAsyncState(readDelegate, callback);
		readDelegate.BeginInvoke(timeout, EndRead, hidAsyncState);
	}

	public async Task<HidDeviceData> ReadAsync(int timeout = 0)
	{
		ReadDelegate readDelegate = Read;
		return await Task<HidDeviceData>.Factory.FromAsync(readDelegate.BeginInvoke, readDelegate.EndInvoke, timeout, null);
	}

	public HidReport ReadReport()
	{
		return ReadReport(0);
	}

	public HidReport ReadReport(int timeout)
	{
		return new HidReport(Capabilities.InputReportByteLength, Read(timeout));
	}

	public void ReadReport(ReadReportCallback callback)
	{
		ReadReport(callback, 0);
	}

	public void ReadReport(ReadReportCallback callback, int timeout)
	{
		ReadReportDelegate readReportDelegate = ReadReport;
		HidAsyncState hidAsyncState = new HidAsyncState(readReportDelegate, callback);
		readReportDelegate.BeginInvoke(timeout, EndReadReport, hidAsyncState);
	}

	public async Task<HidReport> ReadReportAsync(int timeout = 0)
	{
		ReadReportDelegate readReportDelegate = ReadReport;
		return await Task<HidReport>.Factory.FromAsync(readReportDelegate.BeginInvoke, readReportDelegate.EndInvoke, timeout, null);
	}

	public HidReport ReadReportSync(byte reportId)
	{
		byte[] array = new byte[Capabilities.InputReportByteLength];
		array[0] = reportId;
		bool flag = NativeMethods.HidD_GetInputReport(Handle, array, array.Length);
		HidDeviceData deviceData = new HidDeviceData(array, (!flag) ? HidDeviceData.ReadStatus.NoDataRead : HidDeviceData.ReadStatus.Success);
		return new HidReport(Capabilities.InputReportByteLength, deviceData);
	}

	public bool ReadFeatureData(out byte[] data, byte reportId = 0)
	{
		if (_deviceCapabilities.FeatureReportByteLength <= 0)
		{
			data = new byte[0];
			return false;
		}
		data = new byte[_deviceCapabilities.FeatureReportByteLength];
		byte[] array = CreateFeatureOutputBuffer();
		array[0] = reportId;
		IntPtr intPtr = IntPtr.Zero;
		bool flag = false;
		try
		{
			intPtr = ((!IsOpen) ? OpenDeviceIO(_devicePath, 0u) : Handle);
			flag = NativeMethods.HidD_GetFeature(intPtr, array, array.Length);
			if (flag)
			{
				Array.Copy(array, 0, data, 0, Math.Min(data.Length, _deviceCapabilities.FeatureReportByteLength));
			}
		}
		catch (Exception innerException)
		{
			throw new Exception($"Error accessing HID device '{_devicePath}'.", innerException);
		}
		finally
		{
			if (intPtr != IntPtr.Zero && intPtr != Handle)
			{
				CloseDeviceIO(intPtr);
			}
		}
		return flag;
	}

	public bool ReadProduct(out byte[] data)
	{
		data = new byte[254];
		IntPtr intPtr = IntPtr.Zero;
		bool result = false;
		try
		{
			intPtr = ((!IsOpen) ? OpenDeviceIO(_devicePath, 0u) : Handle);
			result = NativeMethods.HidD_GetProductString(intPtr, ref data[0], data.Length);
		}
		catch (Exception innerException)
		{
			throw new Exception($"Error accessing HID device '{_devicePath}'.", innerException);
		}
		finally
		{
			if (intPtr != IntPtr.Zero && intPtr != Handle)
			{
				CloseDeviceIO(intPtr);
			}
		}
		return result;
	}

	public bool ReadManufacturer(out byte[] data)
	{
		data = new byte[254];
		IntPtr intPtr = IntPtr.Zero;
		bool result = false;
		try
		{
			intPtr = ((!IsOpen) ? OpenDeviceIO(_devicePath, 0u) : Handle);
			result = NativeMethods.HidD_GetManufacturerString(intPtr, ref data[0], data.Length);
		}
		catch (Exception innerException)
		{
			throw new Exception($"Error accessing HID device '{_devicePath}'.", innerException);
		}
		finally
		{
			if (intPtr != IntPtr.Zero && intPtr != Handle)
			{
				CloseDeviceIO(intPtr);
			}
		}
		return result;
	}

	public bool ReadSerialNumber(out byte[] data)
	{
		data = new byte[254];
		IntPtr intPtr = IntPtr.Zero;
		bool result = false;
		try
		{
			intPtr = ((!IsOpen) ? OpenDeviceIO(_devicePath, 0u) : Handle);
			result = NativeMethods.HidD_GetSerialNumberString(intPtr, ref data[0], data.Length);
		}
		catch (Exception innerException)
		{
			throw new Exception($"Error accessing HID device '{_devicePath}'.", innerException);
		}
		finally
		{
			if (intPtr != IntPtr.Zero && intPtr != Handle)
			{
				CloseDeviceIO(intPtr);
			}
		}
		return result;
	}

	public bool Write(byte[] data)
	{
		return Write(data, 0);
	}

	public bool Write(byte[] data, int timeout)
	{
		if (IsConnected)
		{
			if (!IsOpen)
			{
				OpenDevice(_deviceReadMode, _deviceWriteMode, _deviceShareMode);
			}
			try
			{
				return WriteData(data, timeout);
			}
			catch
			{
				return false;
			}
		}
		return false;
	}

	public void Write(byte[] data, WriteCallback callback)
	{
		Write(data, callback, 0);
	}

	public void Write(byte[] data, WriteCallback callback, int timeout)
	{
		WriteDelegate writeDelegate = Write;
		HidAsyncState hidAsyncState = new HidAsyncState(writeDelegate, callback);
		writeDelegate.BeginInvoke(data, timeout, EndWrite, hidAsyncState);
	}

	public async Task<bool> WriteAsync(byte[] data, int timeout = 0)
	{
		WriteDelegate writeDelegate = Write;
		return await Task<bool>.Factory.FromAsync(writeDelegate.BeginInvoke, writeDelegate.EndInvoke, data, timeout, null);
	}

	public bool WriteReport(HidReport report)
	{
		return WriteReport(report, 0);
	}

	public bool WriteReport(HidReport report, int timeout)
	{
		return Write(report.GetBytes(), timeout);
	}

	public void WriteReport(HidReport report, WriteCallback callback)
	{
		WriteReport(report, callback, 0);
	}

	public void WriteReport(HidReport report, WriteCallback callback, int timeout)
	{
		WriteReportDelegate writeReportDelegate = WriteReport;
		HidAsyncState hidAsyncState = new HidAsyncState(writeReportDelegate, callback);
		writeReportDelegate.BeginInvoke(report, timeout, EndWriteReport, hidAsyncState);
	}

	public bool WriteReportSync(HidReport report)
	{
		if (report != null)
		{
			byte[] bytes = report.GetBytes();
			return NativeMethods.HidD_SetOutputReport(Handle, bytes, bytes.Length);
		}
		throw new ArgumentException("The output report is null, it must be allocated before you call this method", "report");
	}

	public async Task<bool> WriteReportAsync(HidReport report, int timeout = 0)
	{
		WriteReportDelegate writeReportDelegate = WriteReport;
		return await Task<bool>.Factory.FromAsync(writeReportDelegate.BeginInvoke, writeReportDelegate.EndInvoke, report, timeout, null);
	}

	public HidReport CreateReport()
	{
		return new HidReport(Capabilities.OutputReportByteLength);
	}

	public bool WriteFeatureData(byte[] data)
	{
		if (_deviceCapabilities.FeatureReportByteLength <= 0)
		{
			return false;
		}
		byte[] array = CreateFeatureOutputBuffer();
		Array.Copy(data, 0, array, 0, Math.Min(data.Length, _deviceCapabilities.FeatureReportByteLength));
		IntPtr intPtr = IntPtr.Zero;
		bool result = false;
		try
		{
			intPtr = ((!IsOpen) ? OpenDeviceIO(_devicePath, 0u) : Handle);
			result = NativeMethods.HidD_SetFeature(intPtr, array, array.Length);
		}
		catch (Exception innerException)
		{
			throw new Exception($"Error accessing HID device '{_devicePath}'.", innerException);
		}
		finally
		{
			if (intPtr != IntPtr.Zero && intPtr != Handle)
			{
				CloseDeviceIO(intPtr);
			}
		}
		return result;
	}

	protected static void EndRead(IAsyncResult ar)
	{
		HidAsyncState hidAsyncState = (HidAsyncState)ar.AsyncState;
		ReadDelegate readDelegate = (ReadDelegate)hidAsyncState.CallerDelegate;
		ReadCallback readCallback = (ReadCallback)hidAsyncState.CallbackDelegate;
		HidDeviceData data = readDelegate.EndInvoke(ar);
		readCallback?.Invoke(data);
	}

	protected static void EndReadReport(IAsyncResult ar)
	{
		HidAsyncState hidAsyncState = (HidAsyncState)ar.AsyncState;
		ReadReportDelegate readReportDelegate = (ReadReportDelegate)hidAsyncState.CallerDelegate;
		ReadReportCallback readReportCallback = (ReadReportCallback)hidAsyncState.CallbackDelegate;
		HidReport report = readReportDelegate.EndInvoke(ar);
		readReportCallback?.Invoke(report);
	}

	private static void EndWrite(IAsyncResult ar)
	{
		HidAsyncState hidAsyncState = (HidAsyncState)ar.AsyncState;
		WriteDelegate writeDelegate = (WriteDelegate)hidAsyncState.CallerDelegate;
		WriteCallback writeCallback = (WriteCallback)hidAsyncState.CallbackDelegate;
		bool success = writeDelegate.EndInvoke(ar);
		writeCallback?.Invoke(success);
	}

	private static void EndWriteReport(IAsyncResult ar)
	{
		HidAsyncState hidAsyncState = (HidAsyncState)ar.AsyncState;
		WriteReportDelegate writeReportDelegate = (WriteReportDelegate)hidAsyncState.CallerDelegate;
		WriteCallback writeCallback = (WriteCallback)hidAsyncState.CallbackDelegate;
		bool success = writeReportDelegate.EndInvoke(ar);
		writeCallback?.Invoke(success);
	}

	private byte[] CreateInputBuffer()
	{
		return CreateBuffer(Capabilities.InputReportByteLength - 1);
	}

	private byte[] CreateOutputBuffer()
	{
		return CreateBuffer(Capabilities.OutputReportByteLength - 1);
	}

	private byte[] CreateFeatureOutputBuffer()
	{
		return CreateBuffer(Capabilities.FeatureReportByteLength - 1);
	}

	private static byte[] CreateBuffer(int length)
	{
		byte[] array = null;
		Array.Resize(ref array, length + 1);
		return array;
	}

	private static HidDeviceAttributes GetDeviceAttributes(IntPtr hidHandle)
	{
		NativeMethods.HIDD_ATTRIBUTES attributes = default(NativeMethods.HIDD_ATTRIBUTES);
		attributes.Size = Marshal.SizeOf((object)attributes);
		NativeMethods.HidD_GetAttributes(hidHandle, ref attributes);
		return new HidDeviceAttributes(attributes);
	}

	private static HidDeviceCapabilities GetDeviceCapabilities(IntPtr hidHandle)
	{
		NativeMethods.HIDP_CAPS capabilities = default(NativeMethods.HIDP_CAPS);
		IntPtr preparsedData = default(IntPtr);
		if (NativeMethods.HidD_GetPreparsedData(hidHandle, ref preparsedData))
		{
			NativeMethods.HidP_GetCaps(preparsedData, ref capabilities);
			NativeMethods.HidD_FreePreparsedData(preparsedData);
		}
		return new HidDeviceCapabilities(capabilities);
	}

	private bool WriteData(byte[] data, int timeout)
	{
		if (_deviceCapabilities.OutputReportByteLength <= 0)
		{
			return false;
		}
		byte[] array = CreateOutputBuffer();
		uint lpNumberOfBytesWritten = 0u;
		Array.Copy(data, 0, array, 0, Math.Min(data.Length, _deviceCapabilities.OutputReportByteLength));
		if (_deviceWriteMode == DeviceMode.Overlapped)
		{
			NativeMethods.SECURITY_ATTRIBUTES securityAttributes = default(NativeMethods.SECURITY_ATTRIBUTES);
			NativeOverlapped lpOverlapped = default(NativeOverlapped);
			int dwMilliseconds = ((timeout <= 0) ? (-1) : timeout);
			securityAttributes.lpSecurityDescriptor = IntPtr.Zero;
			securityAttributes.bInheritHandle = true;
			securityAttributes.nLength = Marshal.SizeOf((object)securityAttributes);
			lpOverlapped.OffsetLow = 0;
			lpOverlapped.OffsetHigh = 0;
			lpOverlapped.EventHandle = NativeMethods.CreateEvent(ref securityAttributes, Convert.ToInt32(value: false), Convert.ToInt32(value: true), "");
			try
			{
				NativeMethods.WriteFile(Handle, array, (uint)array.Length, out lpNumberOfBytesWritten, ref lpOverlapped);
			}
			catch
			{
				return false;
			}
			return NativeMethods.WaitForSingleObject(lpOverlapped.EventHandle, dwMilliseconds) switch
			{
				0u => true, 
				258u => false, 
				uint.MaxValue => false, 
				_ => false, 
			};
		}
		try
		{
			NativeOverlapped lpOverlapped2 = default(NativeOverlapped);
			return NativeMethods.WriteFile(Handle, array, (uint)array.Length, out lpNumberOfBytesWritten, ref lpOverlapped2);
		}
		catch
		{
			return false;
		}
	}

	protected HidDeviceData ReadData(int timeout)
	{
		byte[] array = new byte[0];
		HidDeviceData.ReadStatus status = HidDeviceData.ReadStatus.NoDataRead;
		if (_deviceCapabilities.InputReportByteLength > 0)
		{
			uint lpNumberOfBytesRead = 0u;
			array = CreateInputBuffer();
			IntPtr intPtr = Marshal.AllocHGlobal(array.Length);
			if (_deviceReadMode == DeviceMode.Overlapped)
			{
				NativeMethods.SECURITY_ATTRIBUTES securityAttributes = default(NativeMethods.SECURITY_ATTRIBUTES);
				NativeOverlapped lpOverlapped = default(NativeOverlapped);
				int dwMilliseconds = ((timeout <= 0) ? (-1) : timeout);
				securityAttributes.lpSecurityDescriptor = IntPtr.Zero;
				securityAttributes.bInheritHandle = true;
				securityAttributes.nLength = Marshal.SizeOf((object)securityAttributes);
				lpOverlapped.OffsetLow = 0;
				lpOverlapped.OffsetHigh = 0;
				lpOverlapped.EventHandle = NativeMethods.CreateEvent(ref securityAttributes, Convert.ToInt32(value: false), Convert.ToInt32(value: true), string.Empty);
				try
				{
					if (NativeMethods.ReadFile(Handle, intPtr, (uint)array.Length, out lpNumberOfBytesRead, ref lpOverlapped))
					{
						status = HidDeviceData.ReadStatus.Success;
					}
					else
					{
						switch (NativeMethods.WaitForSingleObject(lpOverlapped.EventHandle, dwMilliseconds))
						{
						case 0u:
							status = HidDeviceData.ReadStatus.Success;
							NativeMethods.GetOverlappedResult(Handle, ref lpOverlapped, out lpNumberOfBytesRead, bWait: false);
							break;
						case 258u:
							status = HidDeviceData.ReadStatus.WaitTimedOut;
							array = new byte[0];
							break;
						case uint.MaxValue:
							status = HidDeviceData.ReadStatus.WaitFail;
							array = new byte[0];
							break;
						default:
							status = HidDeviceData.ReadStatus.NoDataRead;
							array = new byte[0];
							break;
						}
					}
					Marshal.Copy(intPtr, array, 0, (int)lpNumberOfBytesRead);
				}
				catch
				{
					status = HidDeviceData.ReadStatus.ReadError;
				}
				finally
				{
					CloseDeviceIO(lpOverlapped.EventHandle);
					Marshal.FreeHGlobal(intPtr);
				}
			}
			else
			{
				try
				{
					NativeOverlapped lpOverlapped2 = default(NativeOverlapped);
					NativeMethods.ReadFile(Handle, intPtr, (uint)array.Length, out lpNumberOfBytesRead, ref lpOverlapped2);
					status = HidDeviceData.ReadStatus.Success;
					Marshal.Copy(intPtr, array, 0, (int)lpNumberOfBytesRead);
				}
				catch
				{
					status = HidDeviceData.ReadStatus.ReadError;
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
		}
		return new HidDeviceData(array, status);
	}

	private static IntPtr OpenDeviceIO(string devicePath, uint deviceAccess)
	{
		return OpenDeviceIO(devicePath, DeviceMode.NonOverlapped, deviceAccess, ShareMode.ShareRead | ShareMode.ShareWrite);
	}

	private static IntPtr OpenDeviceIO(string devicePath, DeviceMode deviceMode, uint deviceAccess, ShareMode shareMode)
	{
		NativeMethods.SECURITY_ATTRIBUTES lpSecurityAttributes = default(NativeMethods.SECURITY_ATTRIBUTES);
		int dwFlagsAndAttributes = 0;
		if (deviceMode == DeviceMode.Overlapped)
		{
			dwFlagsAndAttributes = 1073741824;
		}
		lpSecurityAttributes.lpSecurityDescriptor = IntPtr.Zero;
		lpSecurityAttributes.bInheritHandle = true;
		lpSecurityAttributes.nLength = Marshal.SizeOf((object)lpSecurityAttributes);
		return NativeMethods.CreateFile(devicePath, deviceAccess, (int)shareMode, ref lpSecurityAttributes, 3, dwFlagsAndAttributes, 0);
	}

	private static void CloseDeviceIO(IntPtr handle)
	{
		if (Environment.OSVersion.Version.Major > 5)
		{
			NativeMethods.CancelIoEx(handle, IntPtr.Zero);
		}
		NativeMethods.CloseHandle(handle);
	}

	private void DeviceEventMonitorInserted()
	{
		if (!IsOpen)
		{
			OpenDevice(_deviceReadMode, _deviceWriteMode, _deviceShareMode);
		}
		if (Inserted != null)
		{
			Inserted();
		}
	}

	private void DeviceEventMonitorRemoved()
	{
		if (IsOpen)
		{
			CloseDevice();
		}
		if (Removed != null)
		{
			Removed();
		}
	}

	public void Dispose()
	{
		if (MonitorDeviceEvents)
		{
			MonitorDeviceEvents = false;
		}
		if (IsOpen)
		{
			CloseDevice();
		}
	}
}
