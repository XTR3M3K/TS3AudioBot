// TS3AudioBot - An advanced Musicbot for Teamspeak 3
// Copyright (C) 2017  TS3AudioBot contributors
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the Open Software License v. 3.0
//
// You should have received a copy of the Open Software License along with this
// program. If not, see <https://opensource.org/licenses/OSL-3.0>.

using TS3AudioBot.Audio;
using TS3AudioBot.CommandSystem.CommandResults;

namespace TS3AudioBot.ResourceFactories
{
	public class PlayResource : IAudioResourceResult, IMetaContainer
	{
		public AudioResource AudioResource { get; }
		public string PlayUri { get; }
		public MetaData? Meta { get; set; }

		public PlayResource(string uri, AudioResource baseData, MetaData? meta = null)
		{
			AudioResource = baseData;
			PlayUri = uri;
			Meta = meta;
		}

		public override string ToString() => AudioResource.ToString();
	}
}
