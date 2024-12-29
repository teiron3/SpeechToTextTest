using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.Media.Ocr;
using Windows.Media.SpeechRecognition;
using Windows.Graphics.Imaging;


namespace test3
{
	public static partial class MethodClass
	{
		public static async Task tempMethod(ViewModel vm, object parameter)
		{
			var imageName = vm.filefullpath;
			SoftwareBitmap softwareBitmap;
			vm.x = "read start";
			using (var fileStream = new FileStream(vm.filefullpath, FileMode.Open, FileAccess.Read))
			{
				var decoder = await BitmapDecoder.CreateAsync(fileStream.AsRandomAccessStream());
				softwareBitmap = await decoder.GetSoftwareBitmapAsync();
			}
			vm.x = softwareBitmap.PixelWidth.ToString();
			vm.y = softwareBitmap.PixelHeight.ToString();
			softwareBitmap.Dispose();

		}


		public static async Task speechMethod(ViewModel vm, object parameter)
		{

			try
			{
				vm.speechText = "";

				SpeechRecognizer recognizer = new SpeechRecognizer();

				// (認識結果が生成されるまでの) 無音を検出し、音声入力が続かないと見なす時間の長さ。
				recognizer.Timeouts.InitialSilenceTimeout = TimeSpan.FromSeconds(1.5);
				//認識できないサウンド (雑音) のリッスンを継続し、音声入力が終了したと見なし、認識処理を終了するまでの時間の長さ。
				recognizer.Timeouts.BabbleTimeout = TimeSpan.FromSeconds(5.0);
				//(認識結果が生成された後の) 無音を検出し、音声入力が終了したと見なす時間の長さ。
				recognizer.Timeouts.EndSilenceTimeout = TimeSpan.FromSeconds(1.0);



				await recognizer.CompileConstraintsAsync();

				//イベント処理
				recognizer.HypothesisGenerated += (sender, e) =>
				{
				};

				recognizer.ContinuousRecognitionSession.ResultGenerated += (sender, e) =>
				{
					vm.speechText += e.Result.Text + "\n";
				};

				recognizer.ContinuousRecognitionSession.Completed += async (sender, e) =>
				{
					//recognizer restart
					//await recognizer.ContinuousRecognitionSession.StartAsync();
					//recognizer end
					MessageBox.Show("end");

				};
				//recognizer start
				await recognizer.ContinuousRecognitionSession.StartAsync();

				//timer
				//5秒ごとに終了判定を行う
				DispatcherTimer timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromSeconds(5);
				timer.Tick += async (sender, e) =>
				{
					if (vm.speechText.Length > 50)
					{
						//recognizer end
						await recognizer.ContinuousRecognitionSession.StopAsync();
						await recognizer.ContinuousRecognitionSession.CancelAsync();
						timer.Stop();
					}
				};



			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.HResult + ex.Message + "\n" + ex.ToString());
			}
			// end of 

		}
		// ...existing code...	}
	}
}

