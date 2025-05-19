namespace Framework.Domain.Common;
public static class DomainConsts
{
    public static string LanguageColumnName => "Language";
    public static string ValueTableSuffix => "_values";
    public static string MaterializedTableSuffix => "_Materialized";
    public static string HistoryTableSuffix => "_History";
    public static string RuleOutPutName => "output";
    public static string SubStepPrefixPattern => "#!<";
    public static string SubStepSuffixPattern => ">!#";
    public static string StepActivationTemporaryPrefix => "temp_";

    public static string VariableDatasourceName = "VariableValues";

    public static string ExchangeRatesDatasource = "ExchangeRates";

    public static string ExchangeRatesPathView = "ExchangeRatePaths";

    public static string ExchangeRatesExpandedTable = "ExchangeRatesExpanded";

    public static string CurrencyParameter = "Currency";

}
