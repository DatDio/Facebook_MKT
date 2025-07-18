Facebook_MKT
🎯 Facebook_MKT is a powerful WPF application designed to manage multiple Facebook accounts and pages. It automates tasks such as interacting like a real human, posting content via pages or groups, uploading video Reels, organizing accounts into folders, and tracking account activity — saving time and enhancing your Facebook marketing efficiency.

📌 Features
✅ Manage multiple Facebook accounts in an intuitive interface.

✅ Automatically post content using Pages instead of personal profiles.

✅ Schedule or bulk post to Groups.

✅ Upload and publish Reels videos.

✅ Simulate human-like interactions (like, comment, share...).

✅ Organize accounts by folders and assign Pages or Groups.

✅ Support for multithreaded automation (run multiple accounts in parallel).

✅ Monitor activity and keep track of logs.

🖼️ Screenshots
(You can insert screenshots here if available.)

⚙️ Technologies Used
✅ WPF (.NET Core)

✅ MVVM Pattern

✅ Entity Framework Core – database interaction

✅ MaterialDesign in XAML – modern UI components

✅ Selenium WebDriver – browser automation to mimic human behavior

🚀 Getting Started
1. Clone the Repository
bash
Sao chép
Chỉnh sửa
git clone https://github.com/DatDio/Facebook_MKT.git
2. Open in Visual Studio
Open Facebook_MKT.sln

Restore NuGet packages if not done automatically

3. Initialize Database
Open Package Manager Console and run:

powershell
Sao chép
Chỉnh sửa
Update-Database
Sample data may be seeded automatically if configured in the DbContext.

▶️ How to Use
Launch the application.

Add Facebook accounts to the system.

Assign Pages or Groups to each account.

Perform actions: auto interaction, post content (page/group), upload Reels, monitor activity.

Use the sidebar to navigate through features.

📁 Project Structure
bash
Sao chép
Chỉnh sửa
Facebook_MKT/
├── Facebook_MKT.WPF/        # UI & ViewModels
├── Facebook_MKT.Domain/     # Entity & Interface definitions
├── Facebook_MKT.EFCore/     # Database context and EF logic
├── Facebook_MKT.Core/       # Services, Commands, Helpers
💡 Contributing
We welcome contributions!
Feel free to:

Submit Pull Requests (PR)

Report bugs by opening issues

Suggest new features or improvements

📄 License
This project is licensed under the MIT License.
See LICENSE for more information.

📬 Contact
Author: Đạt Dio

GitHub: https://github.com/DatDio

⭐ If you find this project helpful, consider giving it a star!

