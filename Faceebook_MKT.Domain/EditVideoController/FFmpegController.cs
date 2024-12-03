using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.EditVideoController
{
	public class FFmpegController
	{
		public static bool Add2LayerAndAddText(string videoPath,  string outputPath)
		{

			Process process = new Process();

			// Đặt thông số của tiến trình
			ProcessStartInfo startInfo = new ProcessStartInfo()
			{
				FileName = "cmd.exe",  // Tên tệp thực thi (Command Prompt)

				Arguments = $"/c ffmpeg -i \"{videoPath}\" -stream_loop -1 -i \"layer.mp4\" -stream_loop -1 -i \"layer2.mp4\" -i \"filter_3d.png\" -filter_complex \"[0:v]scale=720:-1,fps=30,eq=contrast=1.1,lut3d='luts/lut1.cube'[v1]; [1:v]colorkey=0x000000:0.2:0.2,format=rgba,colorchannelmixer=aa=0.1,scale=720:650[ckout]; [v1][ckout]overlay=x=(main_w-overlay_w):y=(main_h-overlay_h):shortest=1[main]; [0:v]eq=saturation=2:brightness=0.1,hflip,scale=720:1280,boxblur=luma_radius=min(h\\,w)/5:luma_power=1:chroma_radius=min(cw\\,ch)/5:chroma_power=1[bg]; [bg][main]overlay=(W-w)/2:(H-h)/2,setpts=PTS/1.1,setsar=1:1[v]; [0:a]asetpts=N/SR/TB,atempo=1.1,volume=1.350[a]\" -map \"[v]\" -map \"[a]\" -b:v 10M -vcodec h264 -shortest -metadata album_artist=\"Artist_!RANDOM!\" -metadata album=\"Album_!RANDOM!\" -metadata date=\"2024\" -metadata genre=\"Sample Genre\" \"{outputPath}\"",
				UseShellExecute = false,  // Sử dụng ShellExecute = false để tránh hiển thị cửa sổ Command Prompt
				WindowStyle = ProcessWindowStyle.Hidden,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true  // Tạo cửa sổ mới (ẩn)
			};
			process.StartInfo = startInfo;
			// Khởi động tiến trình
			process.Start();
			process.StandardError.ReadToEnd();
			process.WaitForExit();
			// Kiểm tra mã lỗi của tiến trình
			int exitCode = process.ExitCode;
			if (exitCode != 0)
			{
				// Xử lý lỗi ở đây
				//Console.WriteLine("Tiến trình kết thúc với mã lỗi: " + exitCode);
				string errorMessage = process.StandardError.ReadToEnd();
				Console.WriteLine("Thông báo lỗi: " + errorMessage);

				return false;
			}
			process.Close();
			return true;
		}
	}
}
