using System;
using System.Threading;
using WirecastWrapper;
using System.IO.Ports;
using System.IO;

using System.Collections.Generic;

namespace wireduino
{
	class MainClass
	{
		private static void Log(String message, params object[] arg)
		{
			message = DateTime.Now + " " + message;

			using (StreamWriter w = File.AppendText ("log.txt")) {
				w.WriteLine (message, arg);
				Console.WriteLine (message, arg);
			}
		}

		public static void Main (string[] args)
		{
			Log("Начало работы.");

			Dictionary < int , string > cameras = new Dictionary < int , string > () {
				{ 1, "sasha" },
				{ 2, "lesha" }
			};
				
			Wirecast.StartWirecast();

			WirecastDocument document = Wirecast.Instance.DocumentByIndex(1);
			WirecastLayer layer = document.LayerByName("normal");			
			SerialPort serial = new SerialPort("COM3", 9600);
			WirecastShot lastActiveShot = null;
			bool lastSerialStatus = false;
//			int lastCamerasSum = -1;

			while (true) {
				WirecastShot activeShot;

				try {
					activeShot = layer.ActiveShot;
				} catch {
					Log ("Ошибка получения текущего кадра! Выход.");

					if (serial.IsOpen) {
						Log("Закрываю связь с COM-портом.");
						serial.Close ();
					}

					return;
				}

				if (activeShot != null) {

					if (lastActiveShot == null) {
						Log ("Первоначально выбранный кадр: {0}.", activeShot.ToString ());
						lastActiveShot = activeShot;
					}

					if (!activeShot.Equals(lastActiveShot)) {
						Log (
							"Смена кадра: {0} -> {1}.",
							lastActiveShot,
							activeShot
						);
					
						lastActiveShot = activeShot;
					}

					int camerasSum = 0;

					foreach (KeyValuePair < int , String > camera in cameras)
						if (activeShot.Name.IndexOf(camera.Value) != -1)
							camerasSum += camera.Key;

//					if (lastCamerasSum != camerasSum) {
//						Log (
//							"Код текущего кадра {0}.",
//							camerasSum
//						);
//
//						lastCamerasSum = camerasSum;
//					}

					try
					{
						if (!serial.IsOpen)
							serial.Open ();
					}
					catch {
						if (lastSerialStatus) {
							Log("Не удалось открыть серийный порт!");
							lastSerialStatus = false;
						}

						continue;
					}

					try {
						serial.Write(camerasSum.ToString());
					} catch {
						if (lastSerialStatus) {
							Log("Не удалось передать данные в серийный порт!");
							lastSerialStatus = false;
						}

						continue;
					}

					if (!lastSerialStatus) {
						Log("Связь с серийным портом установлена!");
						lastSerialStatus = true;
					}
				}

				Thread.Sleep (100);
			}
		}
	}
}
