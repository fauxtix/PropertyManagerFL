namespace PropertyManagerFL.Application.ViewModels.GeoApi.Municipios
{

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class ListaMunicipios
    {
        public string codigo { get; set; }
        public string nif { get; set; }
        public string rua { get; set; }
        public string localidade { get; set; }
        public string codigopostal { get; set; }
        public string descrpostal { get; set; }
        public string email { get; set; }
        public string telefone { get; set; }
        public string fax { get; set; }
        public string sitio { get; set; }
        public string areaha { get; set; }
        public string populacao { get; set; }
        public string eleitores { get; set; }
        public string codigoine { get; set; }
        public string nome { get; set; }
        public string distrito { get; set; }
        public Censos2011 censos2011 { get; set; }
        public Censos2021 censos2021 { get; set; }
        public Geojsons geojsons { get; set; }
    }

    public class Censos2011
    {
        public int N_EDIFICIOS_CLASSICOS { get; set; }
        public int N_EDIFICIOS_CLASSICOS_1OU2 { get; set; }
        public int N_EDIFICIOS_CLASSICOS_ISOLADOS { get; set; }
        public int N_EDIFICIOS_CLASSICOS_GEMIN { get; set; }
        public int N_EDIFICIOS_CLASSICOS_EMBANDA { get; set; }
        public int N_EDIFICIOS_CLASSICOS_3OUMAIS { get; set; }
        public int N_EDIFICIOS_CLASSICOS_OUTROS { get; set; }
        public int N_EDIFICIOS_EXCLUSIV_RESID { get; set; }
        public int N_EDIFICIOS_PRINCIPAL_RESID { get; set; }
        public int N_EDIFICIOS_PRINCIP_NAO_RESID { get; set; }
        public int N_EDIFICIOS_1OU2_PISOS { get; set; }
        public int N_EDIFICIOS_3OU4_PISOS { get; set; }
        public int N_EDIFICIOS_CONSTR_ANTES_1919 { get; set; }
        public int N_EDIFICIOS_CONSTR_1919A1945 { get; set; }
        public int N_EDIFICIOS_CONSTR_1946A1960 { get; set; }
        public int N_EDIFICIOS_CONSTR_1961A1970 { get; set; }
        public int N_EDIFICIOS_CONSTR_1971A1980 { get; set; }
        public int N_EDIFICIOS_CONSTR_1981A1990 { get; set; }
        public int N_EDIFICIOS_CONSTR_1991A1995 { get; set; }
        public int N_EDIFICIOS_CONSTR_1996A2000 { get; set; }
        public int N_EDIFICIOS_CONSTR_2001A2005 { get; set; }
        public int N_EDIFICIOS_CONSTR_2006A2011 { get; set; }
        public int N_EDIFICIOS_ESTRUT_BETAO { get; set; }
        public int N_EDIFICIOS_ESTRUT_COM_PLACA { get; set; }
        public int N_EDIFICIOS_ESTRUT_SEM_PLACA { get; set; }
        public int N_EDIFICIOS_ESTRUT_ADOBE_PEDRA { get; set; }
        public int N_EDIFICIOS_ESTRUT_OUTRA { get; set; }
        public int N_ALOJAMENTOS { get; set; }
        public int N_ALOJAMENTOS_FAMILIARES { get; set; }
        public int N_ALOJAMENTOS_FAM_CLASSICOS { get; set; }
        public int N_ALOJAMENTOS_FAM_N_CLASSICOS { get; set; }
        public int N_ALOJAMENTOS_COLECTIVOS { get; set; }
        public int N_CLASSICOS_RES_HABITUAL { get; set; }
        public int N_ALOJAMENTOS_RES_HABITUAL { get; set; }
        public int N_ALOJAMENTOS_VAGOS { get; set; }
        public int N_RES_HABITUAL_COM_AGUA { get; set; }
        public int N_RES_HABITUAL_COM_RETRETE { get; set; }
        public int N_RES_HABITUAL_COM_ESGOTOS { get; set; }
        public int N_RES_HABITUAL_COM_BANHO { get; set; }
        public int N_RES_HABITUAL_AREA_50 { get; set; }
        public int N_RES_HABITUAL_AREA_50_100 { get; set; }
        public int N_RES_HABITUAL_AREA_100_200 { get; set; }
        public int N_RES_HABITUAL_AREA_200 { get; set; }
        public int N_RES_HABITUAL_1_2_DIV { get; set; }
        public int N_RES_HABITUAL_3_4_DIV { get; set; }
        public int N_RES_HABITUAL_ESTAC_1 { get; set; }
        public int N_RES_HABITUAL_ESTAC_2 { get; set; }
        public int N_RES_HABITUAL_ESTAC_3 { get; set; }
        public int N_RES_HABITUAL_PROP_OCUP { get; set; }
        public int N_RES_HABITUAL_ARREND { get; set; }
        public int N_FAMILIAS_CLASSICAS { get; set; }
        public int N_FAMILIAS_INSTITUCIONAIS { get; set; }
        public int N_FAMILIAS_CLASSICAS_1OU2_PESS { get; set; }
        public int N_FAMILIAS_CLASSICAS_3OU4_PESS { get; set; }
        public int N_FAMILIAS_CLASSICAS_NPES65 { get; set; }
        public int N_FAMILIAS_CLASSICAS_NPES14 { get; set; }
        public int N_FAMILIAS_CLASSIC_SEM_DESEMP { get; set; }
        public int N_FAMILIAS_CLASSIC_1DESEMPREG { get; set; }
        public int N_FAMILIAS_CLASS_2MAIS_DESEMP { get; set; }
        public int N_NUCLEOS_FAMILIARES { get; set; }
        public int N_NUCLEOS_1FILH_NAO_CASADO { get; set; }
        public int N_NUCLEOS_2FILH_NAO_CASADO { get; set; }
        public int N_NUCLEOS_FILH_INF_6ANOS { get; set; }
        public int N_NUCLEOS_FILH_INF_15ANOS { get; set; }
        public int N_NUCLEOS_FILH_MAIS_15ANOS { get; set; }
        public int N_INDIVIDUOS_PRESENT { get; set; }
        public int N_INDIVIDUOS_PRESENT_H { get; set; }
        public int N_INDIVIDUOS_PRESENT_M { get; set; }
        public int N_INDIVIDUOS_RESIDENT { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M { get; set; }
        public int N_INDIVIDUOS_RESIDENT_0A4 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_5A9 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_10A13 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_14A19 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_15A19 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_20A24 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_20A64 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_25A64 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_65 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_0A4 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_5A9 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_10A13 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_14A19 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_15A19 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_20A24 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_20A64 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_25A64 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_H_65 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_0A4 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_5A9 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_10A13 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_14A19 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_15A19 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_20A24 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_20A64 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_25A64 { get; set; }
        public int N_INDIVIDUOS_RESIDENT_M_65 { get; set; }
        public int N_INDIV_RESIDENT_N_LER_ESCRV { get; set; }
        public int N_IND_RESIDENT_FENSINO_1BAS { get; set; }
        public int N_IND_RESIDENT_FENSINO_2BAS { get; set; }
        public int N_IND_RESIDENT_FENSINO_3BAS { get; set; }
        public int N_IND_RESIDENT_FENSINO_SEC { get; set; }
        public int N_IND_RESIDENT_FENSINO_POSSEC { get; set; }
        public int N_IND_RESIDENT_FENSINO_SUP { get; set; }
        public int N_IND_RESIDENT_ENSINCOMP_1BAS { get; set; }
        public int N_IND_RESIDENT_ENSINCOMP_2BAS { get; set; }
        public int N_IND_RESIDENT_ENSINCOMP_3BAS { get; set; }
        public int N_IND_RESIDENT_ENSINCOMP_SEC { get; set; }
        public int N_IND_RESIDENT_ENSINCOMP_POSEC { get; set; }
        public int N_IND_RESIDENT_ENSINCOMP_SUP { get; set; }
        public int N_IND_RESID_DESEMP_PROC_1EMPRG { get; set; }
        public int N_IND_RESID_DESEMP_PROC_EMPRG { get; set; }
        public int N_IND_RESID_EMPREGADOS { get; set; }
        public int N_IND_RESID_PENS_REFORM { get; set; }
        public int N_IND_RESID_SEM_ACT_ECON { get; set; }
        public int N_IND_RESID_EMPREG_SECT_PRIM { get; set; }
        public int N_IND_RESID_EMPREG_SECT_SEQ { get; set; }
        public int N_IND_RESID_EMPREG_SECT_TERC { get; set; }
        public int N_IND_RESID_ESTUD_MUN_RESID { get; set; }
        public int N_IND_RESID_TRAB_MUN_RESID { get; set; }
    }

    public class Censos2021
    {
        public int N_EDIFICIOS_CLASSICOS { get; set; }
        public int N_ALOJAMENTOS { get; set; }
        public int N_AGREGADOS { get; set; }
        public int N_INDIVIDUOS_RESIDENT { get; set; }
    }

    public class Centros
    {
        public List<double> centro { get; set; }
        public List<double> centroide { get; set; }
        public List<double> centroDeMassa { get; set; }
        public List<double> centroMedio { get; set; }
        public List<double> centroMediano { get; set; }
    }

    public class Freguesia
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public Geometry geometry { get; set; }
        public List<double> bbox { get; set; }
    }

    public class Geojsons
    {
        public List<Freguesia> freguesias { get; set; }
        public Municipio municipio { get; set; }
    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<double>>> coordinates { get; set; }
    }

    public class Municipio
    {
        public string type { get; set; }
        public Properties properties { get; set; }
        public Geometry geometry { get; set; }
        public List<double> bbox { get; set; }
    }

    public class Properties
    {
        public string Dicofre { get; set; }
        public string Freguesia { get; set; }
        public string Concelho { get; set; }
        public string Distrito { get; set; }
        public string TAA { get; set; }
        public double Area_T_ha { get; set; }
        public double Area_EA_ha { get; set; }
        public string Des_Simpli { get; set; }
        public Centros centros { get; set; }
    }



}
