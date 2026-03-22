using Microsoft.ML.Data;

namespace QUANLYSINHVIEN.Models
{
    public class StudentData
    {
        // Yếu tố 1: Điểm quá trình của môn đang xét
        [LoadColumn(0)]
        public float DiemQT { get; set; }

        // Yếu tố 2: Điểm TB tích lũy (Đại diện cho năng lực học các môn khác)
        [LoadColumn(1)]
        public float GPA { get; set; }

        // Kết quả cần dự báo: Điểm kết thúc môn
        [LoadColumn(2)]
        public float DiemTB { get; set; }
    }

    public class StudentPrediction
    {
        [ColumnName("Score")]
        public float DiemTBDuBao { get; set; }
    }
}