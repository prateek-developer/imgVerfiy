using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using SkiaSharp;

namespace imgVerfiy.Model
{
   
    public class SignatureData
    {
        [ImageType(200, 100)]
        public SKBitmap Image { get; set; }

        [ColumnName("Label")]
        public bool IsMatching { get; set; }
    }

    public class SignaturePrediction
    {
        [ColumnName("PredictedLabel")]
        public bool IsMatching { get; set; }
    }
}
