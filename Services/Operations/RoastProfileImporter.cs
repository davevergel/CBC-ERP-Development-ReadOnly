using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using CbcRoastersErp.Models;
using CbcRoastersErp.Models.Production;

public class RoastProfileImporter
{
    private readonly RoastProfileRepository _repository;

    public RoastProfileImporter(RoastProfileRepository repository)
    {
        _repository = repository;
    }

    public void ImportFromJsonFile(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var roastDate = root.GetProperty("roastisodate").GetString();
            var beanType = root.GetProperty("beans").GetString();
            var title = root.GetProperty("title").GetString();

            var timex = root.GetProperty("timex").EnumerateArray();
            var temp1 = root.GetProperty("temp1").EnumerateArray();
            var temp2 = root.GetProperty("temp2").EnumerateArray();

            var timeList = new List<double>();
            var beanList = new List<double>();
            var envList = new List<double>();

            foreach (var t in timex) timeList.Add(t.GetDouble());
            foreach (var b in temp1) beanList.Add(b.GetDouble());
            foreach (var e in temp2) envList.Add(e.GetDouble());

            int count = Math.Min(timeList.Count, Math.Min(beanList.Count, envList.Count));

            var profile = new RoastProfile
            {
                RoastDate = DateTime.TryParse(roastDate, out var dt) ? dt : DateTime.Now,
                BeanType = beanType,
                Notes = title,
                ProfileFilePath = filePath
            };

            int profileId = _repository.InsertRoastProfile(profile);
            var points = new List<RoastDataPoint>();

            for (int i = 1; i < count; i++)
            {
                double ror = (beanList[i] - beanList[i - 1]) / (timeList[i] - timeList[i - 1]);
                points.Add(new RoastDataPoint
                {
                    RoastProfileId = profileId,
                    TimeSeconds = timeList[i],
                    BeanTemp = beanList[i],
                    EnvironmentTemp = envList[i],
                    ROR = ror
                });
            }

            _repository.InsertRoastDataPoints(points);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to import Artisan roast file: {ex.Message}");
        }
    }
}
