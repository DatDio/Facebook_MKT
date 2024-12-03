using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Faceebook_MKT.Domain.Helpers
{
	public static class ClipboardHelper
	{
		private static readonly object clipboardLock = new object();

		[DllImport("user32.dll")]
		private static extern bool OpenClipboard(IntPtr hWndNewOwner);

		[DllImport("user32.dll")]
		private static extern bool CloseClipboard();

		[DllImport("user32.dll")]
		private static extern bool EmptyClipboard();

		[DllImport("user32.dll")]
		private static extern IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern IntPtr GlobalLock(IntPtr hMem);

		[DllImport("kernel32.dll", SetLastError = true)]
		private static extern bool GlobalUnlock(IntPtr hMem);

		private const uint CF_UNICODETEXT = 13;
		private const uint GMEM_MOVEABLE = 0x0002;

		public static void SetText(string text)
		{
			lock (clipboardLock)
			{
				OpenClipboard(IntPtr.Zero);
				EmptyClipboard();
				IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)((text.Length + 1) * 2));
				IntPtr target = GlobalLock(hGlobal);

				try
				{
					Marshal.Copy(text.ToCharArray(), 0, target, text.Length);
				}
				finally
				{
					GlobalUnlock(hGlobal);
				}

				SetClipboardData(CF_UNICODETEXT, hGlobal);
				CloseClipboard();
			}
		}
	}

}
