using System;
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

namespace WUv4Powertools;

public class ConnectToSharedFolder : IDisposable
{
	[StructLayout(LayoutKind.Sequential)]
	public class NetResource
	{
		public ResourceScope Scope;

		public ResourceType ResourceType;

		public ResourceDisplaytype DisplayType;

		public int Usage;

		public string LocalName;

		public string RemoteName;

		public string Comment;

		public string Provider;
	}

	public enum ResourceScope
	{
		Connected = 1,
		GlobalNetwork,
		Remembered,
		Recent,
		Context
	}

	public enum ResourceType
	{
		Any = 0,
		Disk = 1,
		Print = 2,
		Reserved = 8
	}

	public enum ResourceDisplaytype
	{
		Generic,
		Domain,
		Server,
		Share,
		File,
		Group,
		Network,
		Root,
		Shareadmin,
		Directory,
		Tree,
		Ndscontainer
	}

	private readonly string _networkName;

	public ConnectToSharedFolder(string networkName, NetworkCredential credentials)
	{
		_networkName = networkName;
		int result = WNetAddConnection2(new NetResource
		{
			Scope = ResourceScope.GlobalNetwork,
			ResourceType = ResourceType.Disk,
			DisplayType = ResourceDisplaytype.Share,
			RemoteName = networkName
		}, username: string.IsNullOrEmpty(credentials.Domain) ? credentials.UserName : (credentials.Domain + "\\" + credentials.UserName), password: credentials.Password, flags: 0);
		if (result != 0)
		{
			throw new Win32Exception(result, "Error connecting to remote share");
		}
	}

	~ConnectToSharedFolder()
	{
		Dispose(disposing: false);
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		WNetCancelConnection2(_networkName, 0, force: true);
	}

	[DllImport("mpr.dll")]
	private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);

	[DllImport("mpr.dll")]
	private static extern int WNetCancelConnection2(string name, int flags, bool force);
}
