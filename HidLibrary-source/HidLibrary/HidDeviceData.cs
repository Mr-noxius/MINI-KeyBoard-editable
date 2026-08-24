namespace HidLibrary;

public class HidDeviceData
{
	public enum ReadStatus
	{
		Success,
		WaitTimedOut,
		WaitFail,
		NoDataRead,
		ReadError,
		NotConnected
	}

	public byte[] Data { get; private set; }

	public ReadStatus Status { get; private set; }

	public HidDeviceData(ReadStatus status)
	{
		Data = new byte[0];
		Status = status;
	}

	public HidDeviceData(byte[] data, ReadStatus status)
	{
		Data = data;
		Status = status;
	}
}
