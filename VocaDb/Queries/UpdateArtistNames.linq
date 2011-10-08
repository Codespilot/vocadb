<Query Kind="Statements">
  <Connection>
    <ID>6cfe3d25-404f-4d16-81ec-2b3637d961e2</ID>
    <Persist>true</Persist>
    <Server>JUPITER</Server>
    <SqlSecurity>true</SqlSecurity>
    <Database>VocaloidSite</Database>
    <UserName>VocaDbUser</UserName>
    <Password>AQAAANCMnd8BFdERjHoAwE/Cl+sBAAAAo0jpst+it0upqeZXyd28hQAAAAACAAAAAAAQZgAAAAEAACAAAADyYQd6zW/7vn2EzQohuhtxZyC6izU55mu7t0tZ+f4egwAAAAAOgAAAAAIAACAAAADGAXdKkISiWqMsYEZFeWMfBn1n/w3h5Nw/HlZwXajWThAAAACRrziMi+RjPHnbq4rIw1VgQAAAAD78PwMsMHy1XkpK15uQlavfs6DPEzzSPkwFQBKPMJEcfqtf3Ifyqpfj6OcOD/5q4WNpOt3oThQ+3GnC8KDc010=</Password>
  </Connection>
</Query>

foreach (var artist in Artists) {

	artist.ArtistNames.Add(new ArtistNames { Artist = artist.Id, Language = "Japanese", Value = artist.JapaneseName });
	artist.ArtistNames.Add(new ArtistNames { Artist = artist.Id, Language = "Romaji", Value = artist.RomajiName });
	artist.ArtistNames.Add(new ArtistNames { Artist = artist.Id, Language = "English", Value = artist.EnglishName });

	foreach (var name in artist.ArtistMetadatas.Where(m => m.Type == "AlternateName")) {
	
		artist.ArtistNames.Add(new ArtistNames { Artist = artist.Id, Language = "Japanese", Value = name.Value });
	
	}
		
	artist.ArtistNames.Dump();

}

SubmitChanges();