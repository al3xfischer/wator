﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wator.Core.Services
{
    public class FieldService
    {
        public static IEnumerable<(int RowIndex, int ColumnIndex)> GetSurroundingFields(
            (int RowIndex, int ColumnIndex) position, sbyte[,] field)
        {
            yield return ((position.RowIndex - 1 + field.GetLength(0)) % field.GetLength(0), position.ColumnIndex);
            yield return (position.RowIndex, (position.ColumnIndex + 1) % field.GetLength(1));
            yield return ((position.RowIndex + 1) % field.GetLength(0), position.ColumnIndex);
            yield return (position.RowIndex, (position.ColumnIndex - 1 + field.GetLength(1)) % field.GetLength(1));
        }
    }
}
