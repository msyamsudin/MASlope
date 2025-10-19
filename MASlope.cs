using cAlgo.API;
using cAlgo.API.Indicators;
using System;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MASlope : Indicator
    {
        #region Parameters
        [Parameter("Period", DefaultValue = 14, MinValue = 1)]
        public int Period { get; set; }
        
        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }
        
        [Parameter("Threshold (%)", DefaultValue = 0.1, MinValue = 0)]
        public double Threshold { get; set; }
        
        [Parameter("Show MA Line", DefaultValue = true)]
        public bool ShowMALine { get; set; }
        
        [Parameter("MA Line Period", DefaultValue = 14, MinValue = 1)]
        public int MALinePeriod { get; set; }
        
        [Parameter("MA Line Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MALineType { get; set; }
        
        [Parameter("Source", DefaultValue = DataSourceType.Close)]
        public DataSourceType Source { get; set; }
        
        [Parameter("Smoothing Period", DefaultValue = 1, MinValue = 1)]
        public int SmoothingPeriod { get; set; }
        
        [Parameter("Show Zero Line", DefaultValue = true)]
        public bool ShowZeroLine { get; set; }
        #endregion
        
        #region Outputs
        [Output("Positive Slope", LineColor = "Green", PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries PositiveSlope { get; set; }
        
        [Output("Negative Slope", LineColor = "Red", PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries NegativeSlope { get; set; }
        
        [Output("Moving Average", LineColor = "Blue", PlotType = PlotType.Line, Thickness = 1)]
        public IndicatorDataSeries MovingAverageLine { get; set; }
        
        [Output("Zero Line", LineColor = "Gray", PlotType = PlotType.Line, Thickness = 1, LineStyle = LineStyle.DotsRare)]
        public IndicatorDataSeries ZeroLine { get; set; }
        #endregion
        
        #region Private Fields
        private MovingAverage _ma;
        private MovingAverage _maLine;
        private MovingAverage _smoothMA;
        private IndicatorDataSeries _slopeSeries;
        private IndicatorDataSeries _rawSlopeSeries;
        private const double DOT_SIZE = 0.2;
        #endregion
        
        public enum DataSourceType
        {
            Close,
            Open,
            High,
            Low,
            Median,
            Typical,
            Weighted
        }
        
        protected override void Initialize()
        {
            // Inisialisasi data series
            _rawSlopeSeries = CreateDataSeries();
            _slopeSeries = CreateDataSeries();
            
            // Pilih sumber data
            DataSeries sourceData = GetSourceData();
            
            // Inisialisasi indikator MA
            _ma = Indicators.MovingAverage(sourceData, Period, MAType);
            
            // Smoothing untuk slope jika diperlukan
            if (SmoothingPeriod > 1)
            {
                _smoothMA = Indicators.MovingAverage(_rawSlopeSeries, SmoothingPeriod, MovingAverageType.Simple);
            }
            
            // MA untuk garis slope
            _maLine = Indicators.MovingAverage(_slopeSeries, MALinePeriod, MALineType);
        }
        
        public override void Calculate(int index)
        {
            // Inisialisasi nilai awal
            if (index < Period)
            {
                ResetValues(index);
                return;
            }
            
            // Hitung slope
            double slopePercent = CalculateSlope(index);
            
            // Simpan nilai slope mentah
            _rawSlopeSeries[index] = slopePercent;
            
            // Terapkan smoothing jika diaktifkan
            if (SmoothingPeriod > 1 && index >= Period + SmoothingPeriod - 1)
            {
                slopePercent = _smoothMA.Result[index];
            }
            
            // Simpan slope yang sudah diproses
            _slopeSeries[index] = slopePercent;
            
            // Hitung dan tampilkan MA line
            UpdateMALine(index);
            
            // Update histogram
            UpdateHistogram(index, slopePercent);
            
            // Tampilkan zero line jika diaktifkan
            ZeroLine[index] = ShowZeroLine ? 0 : double.NaN;
        }
        
        #region Helper Methods
        private DataSeries GetSourceData()
        {
            switch (Source)
            {
                case DataSourceType.Open:
                    return Bars.OpenPrices;
                case DataSourceType.High:
                    return Bars.HighPrices;
                case DataSourceType.Low:
                    return Bars.LowPrices;
                case DataSourceType.Median:
                    return Bars.MedianPrices;
                case DataSourceType.Typical:
                    return Bars.TypicalPrices;
                case DataSourceType.Weighted:
                    return Bars.WeightedPrices;
                case DataSourceType.Close:
                default:
                    return Bars.ClosePrices;
            }
        }
        
        private void ResetValues(int index)
        {
            PositiveSlope[index] = 0;
            NegativeSlope[index] = 0;
            _slopeSeries[index] = 0;
            _rawSlopeSeries[index] = 0;
            MovingAverageLine[index] = double.NaN;
            ZeroLine[index] = ShowZeroLine ? 0 : double.NaN;
        }
        
        private double CalculateSlope(int index)
        {
            double currentMA = _ma.Result[index];
            double previousMA = _ma.Result[index - 1];
            
            // Hindari pembagian dengan nol
            if (Math.Abs(previousMA) < double.Epsilon)
                return 0;
            
            // Slope dalam persentase perubahan
            return ((currentMA - previousMA) / previousMA) * 100;
        }
        
        private void UpdateMALine(int index)
        {
            int requiredBars = Period + MALinePeriod - 1;
            if (SmoothingPeriod > 1)
                requiredBars += SmoothingPeriod - 1;
                
            if (ShowMALine && index >= requiredBars)
            {
                MovingAverageLine[index] = _maLine.Result[index];
            }
            else
            {
                MovingAverageLine[index] = double.NaN;
            }
        }
        
        private void UpdateHistogram(int index, double slopePercent)
        {
            double absSlope = Math.Abs(slopePercent);
            
            if (absSlope >= Threshold)
            {
                // Histogram penuh untuk nilai di atas threshold
                if (slopePercent > 0)
                {
                    PositiveSlope[index] = slopePercent;
                    NegativeSlope[index] = 0;
                }
                else
                {
                    PositiveSlope[index] = 0;
                    NegativeSlope[index] = slopePercent;
                }
            }
            else
            {
                // Titik kecil untuk nilai di bawah threshold
                if (slopePercent > 0)
                {
                    PositiveSlope[index] = DOT_SIZE;
                    NegativeSlope[index] = 0;
                }
                else
                {
                    PositiveSlope[index] = 0;
                    NegativeSlope[index] = -DOT_SIZE;
                }
            }
        }
        #endregion
    }
}