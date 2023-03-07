namespace KiotVietTimeSheet.Domain.Engines.SalaryEngine.Abstractions
{
    public interface ISalaryEngine
    {
        decimal CalculateNetSalary();
        decimal CalculateGrossSalary();
    }
}
