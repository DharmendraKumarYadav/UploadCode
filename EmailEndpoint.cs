using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FRT.Properties;

namespace FRT.Messaging
{
	/// <summary>
	/// Endpoint for an e-mail
	/// </summary>
	public sealed class EmailEndpoint
	{
		private static readonly Regex _validEmailRegex = DI.Platform.CreateRegex(@"[a-z0-9]+([-+._][a-z0-9]+){0,2}@.*?(\.(a(?:[cdefgilmnoqrstuwxz]|ero|(?:rp|si)a)|b(?:[abdefghijmnorstvwyz]iz)|c(?:[acdfghiklmnoruvxyz]|at|o(?:m|op))|d[ejkmoz]|e(?:[ceghrstu]|du)|f[ijkmor]|g(?:[abdefghilmnpqrstuwy]|ov)|h[kmnrtu]|i(?:[delmnoqrst]|n(?:fo|t))|j(?:[emop]|obs)|k[eghimnprwyz]|l[abcikrstuvy]|m(?:[acdeghklmnopqrstuvwxyz]|il|obi|useum)|n(?:[acefgilopruz]|ame|et)|o(?:m|rg)|p(?:[aefghklmnrstwy]|ro)|qa|r[eosuw]|s[abcdeghijklmnortuvyz]|t(?:[cdfghjklmnoprtvwz]|(?:rav)?el)|u[agkmsyz]|v[aceginu]|w[fs]|y[etu]|z[amw])\b){1,2}",
			RegexOptions.Singleline | RegexOptions.IgnoreCase, true);
		private static readonly Regex _nonAsciiRegex = DI.Platform.CreateRegex(@"[^\x00-\x7F]",
			RegexOptions.Singleline | RegexOptions.IgnoreCase, true);
		private string _address;
		private string _displayName;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="address"></param>
		/// <param name="displayName"></param>
		public EmailEndpoint(string address = null, string displayName = null)
		{
			if (!string.IsNullOrWhiteSpace(address))
			{
				address = address.Trim();
				if (!_validEmailRegex.IsMatch(address))
				{
					throw new ArgumentException(CommonResources.S_InvalidParameterFormat, nameof(address));
				}
			}

			_displayName = !string.IsNullOrWhiteSpace(displayName) ? _nonAsciiRegex.Replace(displayName.Trim(), string.Empty) : null;
			_displayName = string.IsNullOrWhiteSpace(_displayName) ? null : _displayName;
			_address = string.IsNullOrWhiteSpace(address) ? null : address;
		}

		/// <summary>
		/// Address
		/// </summary>
		public string Address
		{
			get { return _address; }
			set
			{
				var address = value;
				if (!string.IsNullOrWhiteSpace(address))
				{
					address = address.Trim();
					if (!_validEmailRegex.IsMatch(address))
					{
						throw new ArgumentException(CommonResources.S_InvalidParameterFormat, nameof(value));
					}
				}
				_address = string.IsNullOrWhiteSpace(address) ? null : address;
			}
		}

		/// <summary>
		/// Display name
		/// </summary>
		public string DisplayName
		{
			get {  return _displayName ?? _address; }
			set
			{
				_displayName = !string.IsNullOrWhiteSpace(value) ? _nonAsciiRegex.Replace(value.Trim(), string.Empty) : null;
				_displayName = string.IsNullOrWhiteSpace(_displayName) ? null : _displayName;
			}
		}

		/// <summary>
		/// Whether the object is valid
		/// </summary>
		public bool IsValid => _address != null;

		public override string ToString()
		{
			if (IsValid)
			{
				if (_displayName != null)
				{
					return string.Format(CultureInfo.CurrentCulture, "{0} <{1}>", _displayName, _address);
				}
				return _address;
			}
			else
			{
				return "Invalid address";
			}
		}

		/// <summary>
		/// Equality
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			var other = obj as EmailEndpoint;
			if (other == null)
			{
				return false;
			}
			return _displayName.NullableEquals(other._displayName)
			       && _address.NullableEquals(other._address, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Hash code generation
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			StringBuilder sbBuffer = new StringBuilder();
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			if (_displayName != null)
			{
				sbBuffer.Append(_displayName);
			}
			sbBuffer.Append("&*^%$@$#@@$");
			// ReSharper disable once NonReadonlyMemberInGetHashCode
			if (_address != null)
			{
				sbBuffer.Append(_address);
			}
			return sbBuffer.ToString().GetHashCode();
		}
	}
}
