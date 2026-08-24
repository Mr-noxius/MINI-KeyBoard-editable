using System.Threading.Tasks;

namespace HidLibrary;

public class HidFastReadDevice : HidDevice
{
	internal HidFastReadDevice(string devicePath, string description = null)
		: base(devicePath, description)
	{
	}

	public HidDeviceData FastRead()
	{
		return FastRead(0);
	}

	public HidDeviceData FastRead(int timeout)
	{
		try
		{
			return ReadData(timeout);
		}
		catch
		{
			return new HidDeviceData(HidDeviceData.ReadStatus.ReadError);
		}
	}

	public void FastRead(ReadCallback callback)
	{
		FastRead(callback, 0);
	}

	public void FastRead(ReadCallback callback, int timeout)
	{
		ReadDelegate readDelegate = FastRead;
		HidAsyncState hidAsyncState = new HidAsyncState(readDelegate, callback);
		readDelegate.BeginInvoke(timeout, HidDevice.EndRead, hidAsyncState);
	}

	public async Task<HidDeviceData> FastReadAsync(int timeout = 0)
	{
		ReadDelegate readDelegate = FastRead;
		return await Task<HidDeviceData>.Factory.FromAsync(readDelegate.BeginInvoke, readDelegate.EndInvoke, timeout, null);
	}

	public HidReport FastReadReport()
	{
		return FastReadReport(0);
	}

	public HidReport FastReadReport(int timeout)
	{
		return new HidReport(base.Capabilities.InputReportByteLength, FastRead(timeout));
	}

	public void FastReadReport(ReadReportCallback callback)
	{
		FastReadReport(callback, 0);
	}

	public void FastReadReport(ReadReportCallback callback, int timeout)
	{
		ReadReportDelegate readReportDelegate = FastReadReport;
		HidAsyncState hidAsyncState = new HidAsyncState(readReportDelegate, callback);
		readReportDelegate.BeginInvoke(timeout, HidDevice.EndReadReport, hidAsyncState);
	}

	public async Task<HidReport> FastReadReportAsync(int timeout = 0)
	{
		ReadReportDelegate readReportDelegate = FastReadReport;
		return await Task<HidReport>.Factory.FromAsync(readReportDelegate.BeginInvoke, readReportDelegate.EndInvoke, timeout, null);
	}
}
