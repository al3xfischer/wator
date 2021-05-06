
using System.Collections;
using System.Linq;
using Wator.Core.Entities;
using Wator.Core.Services;

var field = new Field(10, 10, Enumerable.Range(0, 100).Select(_ => new Cell()).ToArray());

var service = new FieldService();


var parts = service.Split(10, field);
var merged = service.Merge(parts);

var result = Comparer.Default.Compare(parts.Span[0].Span[5], merged.Span[5]);

var field2 = field with { Cells = merged.ToArray() };


var f = field.GetRows(2, 3);