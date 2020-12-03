using System;
using System.Collections.Generic;

namespace ElasticsearchIntro.Dtos
{
    public class CompanyInformationDto
    {
        public string OrganCode { get; set; }
        public string TaxCode { get; set; }
        public string Ticker { get; set; }
        public string EnterpriseCode { get; set; }
        public string FiinNumber { get; set; }
        public string ComTypeCode { get; set; }
        public string ComGroupCode { get; set; }
        public string OrganName { get; set; }
        public string en_OrganName { get; set; }
        public string OrganShortName { get; set; }
        public string en_OrganShortName { get; set; }
        public string VsicCode { get; set; }
        public string VsicCodeLevel3 { get; set; }
        public string IcbCode { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public DateTime? IncorporationDate { get; set; }
        public int Age => IncorporationDate == null ? 0 : (DateTime.Now.Year - IncorporationDate.Value.Year);
        public string BusTypeCode { get; set; }
        public string BusTypeName { get; set; }
        public string en_BusTypeName { get; set; }
        public string LegalFormCode { get; set; }
        public string LegalFormName { get; set; }
        public string en_LegalFormName { get; set; }
        public string LocationCode { get; set; }
        public string RegisteredAddress { get; set; }
        public string en_RegisteredAddress { get; set; }
        public string PersonCode { get; set; }
        public int? NumberOfEmployee { get; set; }
        public int? NumberOfSubsidiaries { get; set; }
        public decimal? CharterCapital { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? IndexedAt { get; set; }
        public List<FinancialStatement> FinancialStatements { get; set; }
    }
}
