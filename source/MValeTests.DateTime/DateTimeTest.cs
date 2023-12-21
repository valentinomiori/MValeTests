using System;
using System.Collections.Generic;
using System.Linq;

namespace MValeTests.DateTime;

/// <summary>
/// Microsoft reference:
/// 
///     Constructs a DateTimeOffset from a DateTime. For Local and Unspecified kinds,
///     extracts the local offset. For UTC, creates a UTC instance with a zero offset.
///     public DateTimeOffset(DateTime dateTime) {
///         TimeSpan offset;
///         if (dateTime.Kind != DateTimeKind.Utc) {
///             // Local and Unspecified are both treated as Local
///             offset = TimeZoneInfo.GetLocalUtcOffset(dateTime, TimeZoneInfoOptions.NoThrowOnInvalidTime);
///         }
///         else {
///             offset = new TimeSpan(0);
///         }
///         m_offsetMinutes = ValidateOffset(offset);
///         m_dateTime = ValidateDate(dateTime, offset);
///     }
/// </summary>
public class DateTimeTest
{
    [Test]
    public static void Test()
    {
        var ldto = DateTimeOffset.Now;
        var udto = ldto.ToOffset(TimeZoneInfo.Utc.GetUtcOffset(ldto));
        var udtoPlusRtz = udto.ToOffset(
            TimeZoneInfo.GetSystemTimeZones()
            .Where(i => i.GetUtcOffset(udto) != ldto.Offset)
            .OrderBy(i => Random.Shared.NextDouble())
            .First()
            .GetUtcOffset(udto));

        var udtoPlusLtz = udto.ToOffset(TimeZoneInfo.Local.GetUtcOffset(udto));
        Assert.That(ldto, Is.EqualTo(udtoPlusLtz));

        var values = new Dictionary<string, object>()
        {
            [nameof(ldto)] = ldto,
            [nameof(udto)] = udto,
            [nameof(udtoPlusRtz)] = udtoPlusRtz
        };

        foreach ((var key, var value) in values.ToList())
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                values.Add(key + "AsUdt", dateTimeOffset.UtcDateTime);
                values.Add(key + "AsLdt", dateTimeOffset.LocalDateTime);
                values.Add(key + "AsDt", dateTimeOffset.DateTime);
            }
        }

        foreach ((var key, var value) in values.ToList())
        {
            if (value is System.DateTime dateTime)
            {
                values.Add(key + "ToDto", new DateTimeOffset(dateTime));
            }
        }

        foreach (var v in values)
        {
            TestContext.Out.WriteLine(
                v.Key
                + " = "
                + v.Value.GetType().Name
                + " - "
                + ((v.Value is System.DateTime vdt
                    ? (object)vdt.Kind
                    : v.Value is DateTimeOffset vdto
                        ? vdto.Offset
                        : null)
                    ?.ToString() ?? string.Empty)
                + " : "
                + v.Value.ToString());
        }

        var table = from row in (
                from v1 in values
                from v2 in values
                select new { V1 = v1, V2 = v2 })
            orderby row.V1.Key != row.V2.Key
            group row
            by string.Join("", new[] { row.V1.Key, row.V2.Key }.OrderBy(k => k))
            into g
            select g.First();

        foreach (var row in table)
        {
            if (row.V1.Key == row.V2.Key)
            {
                Assert.That(row.V1.Value, Is.SameAs(row.V2.Value));
            }

            TestContext.Out.WriteLine($"{row.V1.Key} == {row.V2.Key} : {object.Equals(row.V1.Value, row.V2.Value)}");
        }
    }
}