using System;
using System.Collections.Generic;
using System.Threading.Tasks;




namespace HolisticWare.HardwareMobile.Bluetooth.LowEnergy
{
	/// <summary>
	/// BLE is used for communication between devices. 
	/// Each device, during communication, can have one of two major roles:
	///		1.	Central (GATT Client).
	///		2.	Peripheral (GATT Server)
	///		
	/// 
	/// http://www.binary-studio.com/2014/08/29/bluetooth-low-energy-for-android-part-1/
	/// 
	/// 
	/// </summary>
	public partial class BluetoothLowEnergy 
		//: IBluetoothLowEnergy
	{
		const int _scanTimeout = 10000;

		/// <summary>
		/// currently scanning for peripheral devices
		/// </summary>
		public bool IsScanning 
		{
			get
			{
				return _isScanning;
			} 
			private set
			{
			}
		}
		protected bool _isScanning = false;


		public static BluetoothLowEnergy Current
		{
			get 
			{ 
				return current; 
			}
		} 
		private static BluetoothLowEnergy current;

	
		static BluetoothLowEnergy ()
		{
			current = new BluetoothLowEnergy();
		}

	}
}

