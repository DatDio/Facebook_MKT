using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Helpers
{
	public class FolderHelper
	{
		public static int GetVideoCountFromFolder(string folderPath)
		{
			try
			{
				// Lấy đường dẫn đầy đủ
				string fullPath = Path.GetFullPath(folderPath);

				// Kiểm tra xem thư mục có tồn tại không
				if (Directory.Exists(fullPath))
				{
					// Lấy tất cả tệp video trong thư mục và đếm
					string[] videoFiles = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories)
						.Where(file => file.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)).ToArray();

					// Trả về số lượng tệp video tìm thấy
					return videoFiles.Length;
				}
				else
				{
					// Thư mục không tồn tại
					return 0;
				}
			}
			catch
			{
				return 0;
			}
		}

		public static async Task<string> GetVideoPathUpReel(string folderPath)
		{
			try
			{
				// Lấy đường dẫn đầy đủ
				string fullPath = Path.GetFullPath(folderPath);

				// Kiểm tra xem thư mục có tồn tại không
				if (Directory.Exists(fullPath))
				{
					// Lấy tất cả tệp video trong thư mục và đếm
					string[] videoFiles = Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories)
						.Where(file => file.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)).ToArray();

					// Kiểm tra xem có video nào không
					if (videoFiles.Length > 0)
					{
						// Tạo đối tượng Random để chọn video ngẫu nhiên
						Random random = new Random();
						int randomIndex = random.Next(videoFiles.Length);
						return Path.GetFullPath(videoFiles[randomIndex]);
					}
					else
					{
						// Nếu không có video nào trong thư mục
						return "";
					}
				}
				else
				{
					// Nếu thư mục không tồn tại
					return "";
				}
			}
			catch
			{
				// Xử lý lỗi
				return "";
			}
		}
		public static async Task<string> GetFolderVideoPathUpReel(string folderPath)
		{
			try
			{
				// Lấy đường dẫn đầy đủ
				string fullPath = Path.GetFullPath(folderPath);

				// Kiểm tra xem thư mục có tồn tại không
				if (Directory.Exists(fullPath))
				{
					// Lấy tất cả thư mục con trong thư mục gốc
					string[] subFolders = Directory.GetDirectories(fullPath, "*", SearchOption.TopDirectoryOnly);

					// Kiểm tra xem có thư mục con nào không
					if (subFolders.Length > 0)
					{
						// Tạo đối tượng Random để chọn thư mục ngẫu nhiên
						Random random = new Random();
						int randomIndex = random.Next(subFolders.Length);

						// Trả về đường dẫn thư mục ngẫu nhiên
						return Path.GetFullPath(subFolders[randomIndex]);
					}
					else
					{
						// Nếu không có thư mục con nào
						return "";
					}
				}
				else
				{
					// Nếu thư mục không tồn tại
					return "";
				}
			}
			catch
			{
				// Xử lý lỗi
				return "";
			}
		}
		
		public static List<string> GetSubdirectories(string parentDirectory)
		{
			try
			{
				// Kiểm tra thư mục cha có tồn tại không
				if (!Directory.Exists(parentDirectory))
				{
					Console.WriteLine($"Thư mục cha không tồn tại: {parentDirectory}");
					return new List<string>();
				}

				// Lấy danh sách tất cả thư mục con
				var subdirectories = Directory.GetDirectories(parentDirectory, "*", SearchOption.TopDirectoryOnly);

				return new List<string>(subdirectories);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy thư mục con: {ex.Message}");
				return new List<string>();
			}
		}
		public static string GetFilesInSubFolders(string directoryPath, string fileType)
		{

			try
			{
				// Kiểm tra thư mục có tồn tại không
				if (!Directory.Exists(directoryPath))
				{
					Console.WriteLine($"Thư mục không tồn tại: {directoryPath}");
					return "";
				}

				// Chọn phần mở rộng dựa trên tham số fileType
				string searchPattern = fileType.ToLower() == "mp4" ? "*.mp4" : fileType.ToLower() == "txt" ? "*.txt" : "";

				// Nếu không phải mp4 hay txt, return danh sách rỗng
				if (string.IsNullOrEmpty(searchPattern))
				{
					Console.WriteLine($"Loại tệp không hợp lệ: {fileType}");
					return "";
				}
				// Lấy tất cả các tệp có phần mở rộng yêu cầu trong mỗi thư mục con
				var filesInSubFolder = Directory.GetFiles(directoryPath, searchPattern);


				if (filesInSubFolder.Length > 0)
				{
					return Path.GetFullPath(filesInSubFolder[0]);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy file: {ex.Message}");
			}

			return "";
		}
		public static string GetTittleFileTxTUpReel(string directoryPath, string fileName)
		{
			try
			{
				// Kiểm tra thư mục có tồn tại không
				if (!Directory.Exists(directoryPath))
				{
					return "";
				}

				// Tìm file tittle.txt trong thư mục con
				var filesInSubFolder = Directory.GetFiles(directoryPath, fileName, SearchOption.AllDirectories);

				if (filesInSubFolder.Length == 0)
				{
					return "";
				}

				// Đọc nội dung của file
				string filePath = filesInSubFolder[0];
				string content = File.ReadAllText(filePath);

				return content;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Lỗi khi lấy file: {ex.Message}");
				return "";
			}
		}

		public static async Task<bool> DeleteFileVideo(string filePath)
		{
			try
			{
				string fullPath = Path.GetFullPath(filePath);
				File.Delete(fullPath);
				return true;
			}
			catch
			{

			}
			return false;
		}
		public static async Task<bool> DeleteFolder(string folderPath)
		{
			try
			{
				// Lấy đường dẫn đầy đủ
				string fullPath = Path.GetFullPath(folderPath);

				for (int i = 1; i <= 2; i++)
				{
					// Kiểm tra xem thư mục có tồn tại không
					if (Directory.Exists(fullPath))
					{
						// Xóa thư mục và tất cả nội dung bên trong
						Directory.Delete(fullPath, recursive: true);
					}
					if (!Directory.Exists(fullPath))
					{
						return true;
					}
					else
					{
						await Task.Delay(500);
					}
				}
			}
			catch
			{
				// Xử lý lỗi
			}
			return false;
		}

		public static double GetDuration(string file)
		{
			//file = $@"E:\Facebook_MKT\Facebook_MKT.WPF\bin\Debug\net8.0-windows\FolderVideoPage\Page Mẹ Và Bé\61567998452847\chời ơi tui cưng xỉu lun á, dô giỏ hàng e oder 1 em bé dề nuôi hem ạ 🤣🤣🤣 #embecuame  #babydangyeu  #viaconyeu  #cucvangcuame  #dochoitreem  #cucvangcuaem❤️  #dothudongchobe.mp4";
			file = new Uri(file).LocalPath;
			if (!File.Exists(file))
			{
				Console.WriteLine("Tệp không tồn tại.");
				return 0;
			}
			try
			{
				// Tạo một đối tượng MediaFile với đường dẫn file video
				var inputFile = new MediaFile { Filename = file };

				// Sử dụng 'using' để đảm bảo tài nguyên được giải phóng sau khi hoàn tất
				using (var engine = new Engine())
				{
					// Lấy thông tin metadata của video
					engine.GetMetadata(inputFile);

					// Kiểm tra xem Metadata và Duration đã được khởi tạo hay chưa
					if (inputFile.Metadata != null && inputFile.Metadata.Duration != null)
					{
						// Trả về độ dài video tính bằng giây
						return inputFile.Metadata.Duration.TotalSeconds;
					}
					else
					{
						// Thông báo nếu Metadata hoặc Duration không tồn tại
						Console.WriteLine("Metadata hoặc Duration không tồn tại cho video.");
						File.Delete(file);
						return 0;
						
					}
				}
			}
			catch (Exception ex)
			{
				// Ghi nhận thông tin lỗi
				Console.WriteLine($"Lỗi khi lấy độ dài video: {ex.Message}");
				return 0;
			}
		}

	}
}
