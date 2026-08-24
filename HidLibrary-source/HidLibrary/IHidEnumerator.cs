using System.Collections.Generic;

namespace HidLibrary;

public interface IHidEnumerator
{
	bool IsConnected(string devicePath);

	IHidDevice GetDevice(string devicePath);

	IEnumerable<IHidDevice> Enumerate();

	IEnumerable<IHidDevice> Enumerate(string devicePath);

	IEnumerable<IHidDevice> Enumerate(int vendorId, params int[] productIds);

	IEnumerable<IHidDevice> Enumerate(int vendorId);
}
