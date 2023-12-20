using System;
using System.Collections.Generic;
using System.Linq;

namespace MValeTests.DateTime;

public class Tests
{
    public class DateTimeTest
    {
        [Test]
        public static void Test()
        {
            var dto = DateTimeOffset.UtcNow;
            var dtoPlus2 = dto.ToOffset(TimeSpan.FromHours(2));
            var udt = dto.UtcDateTime;
            var ldt = dto.LocalDateTime;
            var dt = dto.DateTime;
            var udtToDto = new DateTimeOffset(udt);
            var ldtToDto = new DateTimeOffset(ldt);
            var dtToDto = new DateTimeOffset(dt);

            var values = new Dictionary<string, object>()
            {
                [nameof(dto)] = dto,
                [nameof(dtoPlus2)] = dtoPlus2,
                [nameof(udt)] = udt,
                [nameof(ldt)] = ldt,
                [nameof(dt)] = dt,
                [nameof(udtToDto)] = udtToDto,
                [nameof(ldtToDto)] = ldtToDto,
                [nameof(dtToDto)] = dtToDto
            };

            foreach (var v in values)
            {
                TestContext.Out.WriteLine(v.Key + " = " + v.Value.GetType().Name + " - " + (v.Value is System.DateTime vdt ? (object)vdt.Kind : v.Value is DateTimeOffset vdto ? vdto.Offset : null) + " : " + v.Value.ToString());
            }

            var table = from v1 in values
                        from v2 in values
                        orderby v1.Key != v2.Key
                        select new { V1 = v1, V2 = v2 };

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
}