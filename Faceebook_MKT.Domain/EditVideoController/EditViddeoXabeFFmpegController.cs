using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Faceebook_MKT.Domain.EditVideoController
{
	public class EditViddeoXabeFFmpegController
	{
		string ffmpegPath;

		// Static constructor: chỉ chạy một lần khi lớp được sử dụng lần đầu
		public EditViddeoXabeFFmpegController()
		{
			// Cố gắng lấy đường dẫn tuyệt đối tới ffmpeg.exe
			//ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), "ffmpeg.exe");
			ffmpegPath = @"E:\Facebook_MKT\Faceebook_MKT.Domain\bin\Debug\net8.0\FFMPEG";

			// Kiểm tra nếu ffmpeg.exe tồn tại tại vị trí này
			//if (!File.Exists(ffmpegPath))
			//{
			//	throw new FileNotFoundException($"Không tìm thấy ffmpeg.exe tại {ffmpegPath}");
			//}

			// Đặt đường dẫn cho FFmpeg
			FFmpeg.SetExecutablesPath(ffmpegPath); // Đặt đường dẫn FFmpeg
		}

		// Hàm zoom video và chèn text
		public async Task<bool> ProcessVideo(string inputPath, string outputPath, string text)
		{
			 //inputPath = @"""E:\Facebook_MKT\Facebook_MKT.WPF\bin\Debug\net8.0-windows\FolderVideoPage\Page Mỹ Phẩm\61557697067796\testmyopham\videotesstedit.mp4""";
			if (!File.Exists(inputPath))
			{
				return false;
			}
			try
			{
				var conversion = await FFmpeg.Conversions.New()
					.AddParameter($"-i {inputPath}") // Đường dẫn video gốc
					.AddParameter("-vf \"scale=iw*1.1:ih*1.1,drawtext=text='" + text + "':fontcolor=white:fontsize=24:x=10:y=H-th-10\"") // Zoom và chèn text
					.SetOutput(outputPath) // Đường dẫn video sau khi xử lý
					.Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi xử lý video: {ex.Message}");
				return false;
			}
			return true;
		}
	}



}
