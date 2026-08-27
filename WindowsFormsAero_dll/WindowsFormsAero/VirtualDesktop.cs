using System;

namespace WindowsFormsAero;

/// <summary>
/// Represents a virtual desktop instance.
/// </summary>
public struct VirtualDesktop : IEquatable<VirtualDesktop>
{
	/// <summary>
	/// Gets the virtual desktop's ID.
	/// </summary>
	public Guid Id { get; }

	internal VirtualDesktop(Guid id)
	{
		Id = id;
	}

	public override bool Equals(object obj)
	{
		if (obj is VirtualDesktop)
		{
			return Equals(obj);
		}
		return false;
	}

	public bool Equals(VirtualDesktop other)
	{
		return Id == other.Id;
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}
}
