// TS3AudioBot - An advanced Musicbot for Teamspeak 3
// Copyright (C) 2016  TS3AudioBot contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace TS3AudioBot.Sessions
{
	using Helper;
	using System;
	using System.Collections.Generic;
	using System.Security.Cryptography;
	using TS3Client.Messages;

	public class SessionManager
	{
		private static readonly TimeSpan DefaultApiTimeout = TimeSpan.FromDays(1);
		const string tokenFormat = "{0}:{1}";
		private static readonly MD5 Md5Hash = MD5.Create();

		// Map: Uid => UserSession
		private readonly Dictionary<string, UserSession> openSessions;

		public SessionManager()
		{
			Util.Init(ref openSessions);
		}

		public R<UserSession> CreateSession(MainBot bot, ClientData client)
		{
			if (bot == null)
				throw new ArgumentNullException(nameof(bot));

			lock (openSessions)
			{
				UserSession session;
				if (openSessions.TryGetValue(client.Uid, out session))
					return session;

				Log.Write(Log.Level.Debug, "SM User {0} created session with the bot", client.NickName);
				session = new UserSession(bot, client);
				openSessions.Add(client.Uid, session);
				return session;
			}
		}

		public R<UserSession> GetSession(string uid)
		{
			lock (openSessions)
			{
				UserSession session;
				if (openSessions.TryGetValue(uid, out session))
					return session;
				else
					return "Session not found";
			}
		}

		public void RemoveSession(string uid)
		{
			lock (openSessions)
			{
				UserSession session;
				if (uid == null)
				{
					Log.Write(Log.Level.Warning, "Null remove session");
					return;
				}
				if (openSessions.TryGetValue(uid, out session))
				{
					if (session.HasActiveToken)
						openSessions.Remove(uid);
				}
			}
		}

		public R<string> GetToken(UserSession session) => GetToken(session, DefaultApiTimeout);
		public R<string> GetToken(UserSession session, TimeSpan timeout)
		{
			if (session.Token == null)
				session.Token = new UserToken() { UserUid = session.Client.Uid };

			session.Token.ApiToken = GenToken();
			var newTimeout = Util.GetNow() + timeout;
			if (newTimeout > session.Token.ApiTokenTimeout)
				session.Token.ApiTokenTimeout = newTimeout;

			return R<string>.OkR(string.Format(tokenFormat, session.Client.Uid, session.Token.ApiToken));
		}

		private static string GenToken(int len = 32)
		{
			const string alph = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			var arr = new char[len];
			for (int i = 0; i < arr.Length; i++)
				arr[i] = alph[Util.Random.Next(0, alph.Length)];
			return new string(arr);
		}
	}
}