using System;
using CoreBluetooth;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreFoundation;

namespace HolisticWare.HardwareMobile.Bluetooth.LowEnergy
{
	/// <summary>
	/// Bluetooth Low Energy connectivity class. 
	/// Adds functionality to the  CoreBluetooth Manager to track discovered 
	/// devices, scanning state, and automatically stops scanning after a timeout period.
	/// </summary>
	public partial class BluetoothLowEnergy// : CBCentralManagerDelegate
	{
		// event declarations
		public event EventHandler<CBDiscoveredPeripheralEventArgs> DeviceDiscovered = delegate {};
		public event EventHandler<CBPeripheralEventArgs> DeviceConnected = delegate {};
		public event EventHandler<CBPeripheralErrorEventArgs> DeviceDisconnected = delegate {};
		public event EventHandler ScanTimeoutElapsed = delegate {};

		/// <summary>
		/// Gets the discovered peripherals.
		/// </summary>
		/// <value>The discovered peripherals.</value>
		public List<CBPeripheral> DiscoveredDevices
		{
			get { return _discoveredDevices; }
		}
		List<CBPeripheral> _discoveredDevices = new List<CBPeripheral>();

		/// <summary>
		/// Gets the connected peripherals.
		/// </summary>
		/// <value>The discovered peripherals.</value>
		public List<CBPeripheral> ConnectedDevices
		{
			get { return _connectedDevices; }
		}
		List<CBPeripheral> _connectedDevices = new List<CBPeripheral>();

		public CBCentralManager CentralBleManager
		{
			get { return cb_central_manager; }
		}
		CBCentralManager cb_central_manager;


		static BluetoothLowEnergy ()
		{
			current = new BluetoothLowEnergy();
		}

		protected BluetoothLowEnergy ()
		{
			cb_central_manager = new CBCentralManager (DispatchQueue.CurrentQueue);
			cb_central_manager.DiscoveredPeripheral += (object sender, CBDiscoveredPeripheralEventArgs e) => {
				Console.WriteLine ("DiscoveredPeripheral: {0}", e.Peripheral.Name);
				_discoveredDevices.Add (e.Peripheral);
				DeviceDiscovered(this, e);
			};

			cb_central_manager.UpdatedState += (object sender, EventArgs e) => {
				Console.WriteLine ("UpdatedState: {0}", cb_central_manager.State);
			};


			cb_central_manager.ConnectedPeripheral += (object sender, CBPeripheralEventArgs e) => {
				Console.WriteLine ("ConnectedPeripheral: {0}", e.Peripheral.Name);

				// when a peripheral gets connected, add that peripheral to our running list of connected peripherals
				if(!_connectedDevices.Contains(e.Peripheral) ) {
					_connectedDevices.Add (e.Peripheral );
				}			

				// raise our connected event
				DeviceConnected ( sender, e);
			
			};

			cb_central_manager.DisconnectedPeripheral += (object sender, CBPeripheralErrorEventArgs e) => {
				Console.WriteLine ("DisconnectedPeripheral: " + e.Peripheral.Name);

				// when a peripheral disconnects, remove it from our running list.
				if (_connectedDevices.Contains (e.Peripheral) ) {
					_connectedDevices.Remove ( e.Peripheral);
				}

				// raise our disconnected event
				DeviceDisconnected (sender, e);
			};
		}

		/// <summary>
		/// Begins the scanning for bluetooth LE devices. Automatically called after 10 seconds
		/// to prevent battery drain.
		/// </summary>
		/// <returns>The scanning for devices.</returns>
		public async void StartScanningForDevices()
		{
			Console.WriteLine ("BluetoothLEManager: Starting a scan for devices.");

			// clear out the list
			_discoveredDevices = new List<CBPeripheral> ();

			// start scanning
			IsScanning = true;
			cb_central_manager.ScanForPeripherals ((CBUUID[])null);

			// in 10 seconds, stop the scan
			await Task.Delay (10000);

			// if we're still scanning
			if (IsScanning) {
				Console.WriteLine ("BluetoothLEManager: Scan timeout has elapsed.");
				cb_central_manager.StopScan ();
				ScanTimeoutElapsed (this, new EventArgs ());
			}
		}

		/// <summary>
		/// Stops the Central Bluetooth Manager from scanning for more devices. Automatically
		/// called after 10 seconds to prevent battery drain. 
		/// </summary>
		public void StopScanningForDevices()
		{
			Console.WriteLine("BluetoothLEManager: Stopping the scan for devices.");
			IsScanning = false;
			cb_central_manager.StopScan();
		}

		//TODO: rename to DisconnectDevice
		public void DisconnectPeripheral (CBPeripheral peripheral)
		{
			cb_central_manager.CancelPeripheralConnection (peripheral);
		}
	}
}