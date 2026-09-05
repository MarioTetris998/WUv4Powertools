using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WUv4Powertools;

// Asks a server whether a download is there and how big it is.
//
// These files live on the update mirrors of a service that stopped in 2011, and on the copies people
// keep of it. Their certificates are what you would expect: issued for a different name, or expired,
// or signed by nobody in particular. download.windowsupdate.com over https is one of them. Checking
// a link on any of those came back saying the trust relationship could not be established, and the
// whole point of the check is whether the file is there.
//
// So the certificate is not weighed. Nothing is sent to these servers and nothing but the length of
// the reply is read, so there is nothing for a false certificate to take. The protocols are still
// the newer ones, set at startup, since some of these hosts refuse the old ones outright.
public static class LinkCheck
{
	// How long to wait before deciding a server is not going to answer. The default is 100 seconds,
	// which reads as the application having hung.
	private const int WaitMs = 15000;

	// The size of the file the address names, or -1 when the server does not say.
	// Throws when the address cannot be reached at all, which is what the caller reports.
	public static long SizeOf(string address)
	{
		if (string.IsNullOrEmpty(address)) throw new ArgumentException("No address was given.", "address");

		using (HttpWebResponse response = Ask(address))
		{
			return response.ContentLength;
		}
	}

	// Whether the address can be reached, with no exception either way.
	public static bool Reachable(string address)
	{
		try
		{
			SizeOf(address);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static HttpWebResponse Ask(string address)
	{
		HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(address));
		request.Timeout = WaitMs;
		request.ReadWriteTimeout = WaitMs;

		// Only for this request. The setting is left alone everywhere else in the application.
		request.ServerCertificateValidationCallback = AcceptAnyCertificate;

		// A few of these mirrors turn away a request that names no browser.
		request.UserAgent = "Windows Update v4.0 PowerTools";
		return (HttpWebResponse)request.GetResponse();
	}

	private static bool AcceptAnyCertificate(object sender, X509Certificate certificate,
		X509Chain chain, SslPolicyErrors errors)
	{
		return true;
	}
}
