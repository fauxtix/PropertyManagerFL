using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ObjectsComparer;
using PropertyManagerFL.Application.Interfaces.Services.AppManager;
using PropertyManagerFL.Application.Interfaces.Services.Validation;
using PropertyManagerFL.Application.ViewModels.Fracoes;
using PropertyManagerFL.Application.ViewModels.Imoveis;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;
using System.Collections.Immutable;
using static PropertyManagerFL.Application.Shared.Enums.AppDefinitions;


namespace PropertyManagerFL.UI.Pages.ComponentsBase
{

    /// <summary>
    /// Gestão de imóveis
    /// </summary>
    public class ImoveisBase : ComponentBase, IDisposable
    {
        [Inject] public IImovelService? propertiesService { get; set; }
        [Inject] public IFracaoService? unitsService { get; set; }
        [Inject] protected IValidationService? validatorService { get; set; }
        [Inject] public IWebHostEnvironment? hostingEnvironment { get; set; }
        [Inject] public IStringLocalizer<App>? L{ get; set; }

        protected IEnumerable<ImovelVM>? properties { get; set; }
        protected IEnumerable<FracaoVM>? units { get; set; }
        protected ImovelVM? SelectedProperty { get; set; }
        protected FracaoVM? SelectedUnit { get; set; }
        protected ImovelVM? OriginalPropertyData { get; set; }
        protected FracaoVM? OriginalUnitData { get; set; }
        protected OpcoesRegisto RecordMode { get; set; }
        protected int PropertyId { get; set; }
        protected int UnitId { get; set; }
        protected string? NewCaption { get; set; }
        protected string? EditCaption { get; set; }
        protected string? DeleteCaption { get; set; }

        protected string? DeletePropertyCaption;
        protected string? DeleteUnitCaption;
        protected bool DeletePropertyVisibility { get; set; }
        protected bool DeleteUnitVisibility { get; set; }

        protected bool AddEditPropertyVisibility { get; set; }
        protected bool AddEditUnitVisibility { get; set; }
        protected bool WarningVisibility { get; set; }
        protected string? WarningMessage { get; set; }

        public DialogEffect Effect = DialogEffect.Zoom;
        protected SfSpinner? SpinnerObj { get; set; }
        protected SfGrid<ImovelVM>? propertiesGridObj { get; set; }
        protected SfGrid<FracaoVM>? unitsObj { get; set; }
        protected SfToast? ToastObj { get; set; }



        protected bool IsDirty = false;
        protected List<string> ValidationsMessages = new();
        public bool ErrorVisibility { get; set; } = false;

        protected Modules modulo { get; set; }
        protected AlertMessageType? alertMessageType = AlertMessageType.Info;

        protected string? ToastTitle;
        protected string? ToastMessage;
        protected string? ToastCss;
        protected string? ToastIcon;


        protected List<NovaImagemFracao>? UnitImages { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ToastTitle = "";
            ToastMessage = "";
            ToastCss = "";
            ToastIcon = "";

            OriginalPropertyData = new();
            OriginalUnitData = new();

            AddEditPropertyVisibility = false;
            AddEditUnitVisibility = false;

            DeletePropertyVisibility = false;
            DeleteUnitVisibility = false;

            WarningVisibility = false;
            WarningMessage = "";

            PropertyId = 0;
            UnitId = 0;

            IsDirty = false;

            properties = await GetAllProperties();
            if (properties is null)
            {
                WarningMessage = L["TituloSemDadosParaMostrar"];
                WarningVisibility = true;
            }
        }

        /// <summary>
        /// Get all properties
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ImovelVM>> GetAllProperties()
        {
            IEnumerable<ImovelVM>? propertiesList = (await propertiesService!.GetAll()).ToList();
            propertiesList.OrderByDescending(p => p.Id);
            return propertiesList;
        }
        /// <summary>
        /// Get specific property units
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FracaoVM> GetProperty_Units(int id)
        {
            try
            {
                IEnumerable<FracaoVM> propertyUnitsList = Task.Run(async () => await unitsService!.GetFracoes_Imovel(PropertyId)).Result;
                propertyUnitsList.OrderByDescending(p => p.Id).ToList();
                return propertyUnitsList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected async Task PropertyDoubleClickHandler(RecordDoubleClickEventArgs<ImovelVM> args)
        {
            PropertyId = args.RowData.Id;
            modulo = Modules.Imoveis;
            SelectedProperty = await propertiesService!.GetImovel_ById(PropertyId);
            OriginalPropertyData = await propertiesService.GetImovel_ById(PropertyId); // TODO  should used 'Clone/MemberWise'?

            AddEditPropertyVisibility = true;
            EditCaption = L["EditMsg"] + " " + L["TituloImovel"];
            RecordMode = OpcoesRegisto.Gravar;
            units = await unitsService!.GetFracoes_Imovel(PropertyId);
        }

        protected async Task ToolbarClickHandler(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "Properties_Grid_pdfexport")
            {
                await PdfExport(1);
            }
            if (args.Item.Text == "Expand all")
            {
                await propertiesGridObj.ExpandAllDetailRowAsync();
            }
            if (args.Item.Text == "Collapse all")
            {
                await propertiesGridObj.CollapseAllDetailRowAsync();
            }
        }

        protected List<PdfHeaderFooterContent> FooterContent = new List<PdfHeaderFooterContent>
        {
                new PdfHeaderFooterContent()
                {
                    Type = ContentType.Text, Value = $"Emissão: {DateTime.Now.ToShortDateString()}",
                    Position = new PdfPosition() { X =0, Y = 20
                 },
                 Style = new PdfContentStyle()
                 {
                        TextBrushColor = "#C0C0C0", FontSize = 12 },
                        PageNumberType = PdfPageNumberType.Numeric
                 },
                new PdfHeaderFooterContent()
                {
                    Type = ContentType.PageNumber,
                    PageNumberType = PdfPageNumberType.Numeric,
                    Position = new PdfPosition() { X = 550, Y = 20
                },
                Style = new PdfContentStyle()
                {
                    TextBrushColor = "#000000",
                    FontSize = 12,
                    HAlign = PdfHorizontalAlign.Center }
            }
        };

        protected async Task PdfExport(int unitId)
        {

            WarningMessage = "";
            WarningVisibility = false;

            PdfExportProperties ExportProperties = new PdfExportProperties();

            PdfTheme Theme = new PdfTheme();

            PdfBorder HeaderBorder = new PdfBorder();
            HeaderBorder.Color = "#C0C0C0";

            PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
            {
                FontColor = "#C0C0C0",
                FontName = "Calibri",
                FontSize = 11,
                Bold = true,
                Border = HeaderBorder
            };
            Theme.Header = HeaderThemeStyle;

            PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
            {
                FontColor = "#000000",
                FontName = "Calibri",
                FontSize = 9
            };
            Theme.Record = RecordThemeStyle;

            PdfThemeStyle CaptionThemeStyle = new PdfThemeStyle()
            {
                FontColor = "#00FF00",
                FontName = "Calibri",
                FontSize = 10,
                Bold = true

            };
            Theme.Caption = CaptionThemeStyle;

            var selectedRecord = await propertiesGridObj.GetSelectedRecordsAsync();

            int _propertyId;
            string? propertyAddress;
            string? propertyPhoto;
            string? propertyDescription;
            string? propertyCountyParish;
            string? propertyCounty;
            string? propertyDoorNumber;
            string? propertyConstructionYear;
            string? propertyConservationState;
            string? propertyHasElevator;

            if (selectedRecord.Count() > 0)
            {
                _propertyId = selectedRecord.First().Id;
                propertyDescription = selectedRecord.First().Descricao;
                propertyAddress = selectedRecord.First().Morada;
                propertyCountyParish = selectedRecord.First().FreguesiaImovel;
                propertyCounty = selectedRecord.First().ConcelhoImovel;
                propertyDoorNumber = selectedRecord.First().Numero;
                propertyConstructionYear = selectedRecord.First().AnoConstrucao;
                propertyConservationState = selectedRecord.First().EstadoConservacao;
                propertyHasElevator = selectedRecord.First().Elevador ? "Sim" : "Não";
                propertyPhoto = Path.Combine(hostingEnvironment!.WebRootPath, "uploads", "properties", selectedRecord!.First().Foto!);

                byte[] imageArray = System.IO.File.ReadAllBytes(propertyPhoto);
                string base64ImageRepresentation = Convert.ToBase64String(imageArray);

                List<PdfHeaderFooterContent> PropertyHeaderContent = new List<PdfHeaderFooterContent>
                {
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="RESUMO DO IMÓVEL", Position = new PdfPosition() { X = 220, Y = 0 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 20} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Descrição", Position = new PdfPosition() { X = 300, Y = 50 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Construção", Position = new PdfPosition() { X = 600, Y = 50 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value=propertyDescription, Position = new PdfPosition() { X = 300, Y = 65 }, Style = new PdfContentStyle() { TextBrushColor = "#000000"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value= propertyConstructionYear, Position = new PdfPosition() { X = 600, Y = 65 }, Style = new PdfContentStyle() { TextBrushColor = "#000000"} },

                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Morada", Position = new PdfPosition() { X = 300, Y = 90 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Porta", Position = new PdfPosition() { X = 600, Y = 90 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value=propertyAddress, Position = new PdfPosition() { X = 300, Y = 105 }, Style = new PdfContentStyle() { TextBrushColor = "#000000"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value= propertyDoorNumber, Position = new PdfPosition() { X = 600, Y = 105 }, Style = new PdfContentStyle() { TextBrushColor = "#000000"} },

                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Freguesia", Position = new PdfPosition() { X = 300, Y = 130 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Concelho", Position = new PdfPosition() { X = 600, Y = 130 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value=propertyCountyParish, Position = new PdfPosition() { X = 300, Y = 145 }, Style = new PdfContentStyle() { TextBrushColor = "#000000"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value=propertyCounty, Position = new PdfPosition() { X = 600, Y = 145 }, Style = new PdfContentStyle() { TextBrushColor = "#000000"} },

                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Elevador", Position = new PdfPosition() { X = 600, Y = 210 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value=propertyHasElevator, Position = new PdfPosition() { X = 600, Y = 225 }, Style = new PdfContentStyle() { TextBrushColor = "#000000"} },

                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Estado Conservação", Position = new PdfPosition() { X = 300, Y = 250 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878"} },
                     new PdfHeaderFooterContent() { Type = ContentType.Text, Value=propertyConservationState, Position = new PdfPosition() { X = 300, Y = 265 }, Style = new PdfContentStyle() { TextBrushColor = "#000000"} },

                     new PdfHeaderFooterContent()
                     {
                         Type = ContentType.Image,
                         Src = base64ImageRepresentation,
                         Position = new PdfPosition() { X = 20, Y = 50 },
                         Size= new PdfSize()
                         {
                             Height= 150, Width=200
                         }
                     },
                    new PdfHeaderFooterContent() { Type = ContentType.Text, Value="FL Soft Systems", Position = new PdfPosition() { X = 20, Y = 255 }, Style = new PdfContentStyle() { TextBrushColor = "#C67878", FontSize = 15} },
                    new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Rua José Joaquim Marques, 4 3 E", Position = new PdfPosition() { X = 20, Y = 280 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 10} },
                    new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Tel +351 937111222", Position = new PdfPosition() { X = 20, Y = 295 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 10} },
                    new PdfHeaderFooterContent() { Type = ContentType.Text, Value="Montijo - Portugal", Position = new PdfPosition() { X = 20, Y = 310 }, Style = new PdfContentStyle() { TextBrushColor = "#000000", FontSize = 10} },


                };

                List<PdfHeaderFooterContent> PropertyFooterContent = new List<PdfHeaderFooterContent>
                {
                    new PdfHeaderFooterContent()
                    {
                        Type = ContentType.Text, Value = $"Emissão: {DateTime.Now.ToShortDateString()}",
                        Position = new PdfPosition() { X =0, Y = 20
                     },
                     Style = new PdfContentStyle()
                     {
                            TextBrushColor = "#C0C0C0", FontSize = 12 },
                            PageNumberType = PdfPageNumberType.Numeric
                     },
                    new PdfHeaderFooterContent()
                    {
                        Type = ContentType.PageNumber,
                        PageNumberType = PdfPageNumberType.Numeric,
                        Position = new PdfPosition() { X = 550, Y = 20
                    },
                    Style = new PdfContentStyle()
                    {
                        TextBrushColor = "#000000",
                        FontSize = 12,
                        HAlign = PdfHorizontalAlign.Center }
                    },
                };

                ExportProperties.Header = new PdfHeader()
                {
                    FromTop = 0,
                    Height = 360,
                    Contents = PropertyHeaderContent
                };

                ExportProperties.Footer = new PdfFooter()
                {
                    Height = 40,
                    Contents = PropertyFooterContent,
                    FromBottom = 5
                };


                var detailDataSource = await unitsService!.GetFracoes_Imovel(_propertyId); // grid detail records (propertyUnits)
                ExportProperties.DataSource = detailDataSource;
            }
            else
            {
                WarningMessage = "Selecione Imóvel, p.f.";
                WarningVisibility = true;
                return;
            }

            ExportProperties.Theme = Theme;
            ExportProperties.DisableAutoFitWidth = true;
            ExportProperties.IsRepeatHeader = true;
            ExportProperties.HierarchyExportMode = HierarchyExportMode.Expanded;
            ExportProperties.FileName = $"Properties_{DateTime.Now.ToFileTime()}.pdf";

            ExportProperties.Columns = new List<GridColumn>()
            {
                #pragma warning disable BL0005
                new GridColumn(){ Field="Descricao", HeaderText="Descrição", TextAlign=TextAlign.Left, Width="200"},
                new GridColumn(){ Field="Andar", HeaderText="Andar",   TextAlign=TextAlign.Center, Width="50"},
                new GridColumn(){ Field="Lado", HeaderText="Lado",   TextAlign=TextAlign.Center, Width="50"},
                new GridColumn(){ Field="ValorRenda", HeaderText="Renda", TextAlign=TextAlign.Right, Width="60"},
                new GridColumn(){ Field="TipoPropriedade", HeaderText="Tipo", TextAlign=TextAlign.Left, Width="150"}
            };

            await propertiesGridObj!.ExportToPdfAsync(ExportProperties);

        }



        /// <summary>
        /// Command buttons handler
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task OnPropertyCommandClicked(CommandClickEventArgs<ImovelVM> args)
        {
            PropertyId = args.RowData.Id;
            modulo = Modules.Imoveis;
            DeletePropertyCaption = "";
            DeleteUnitCaption = "";

            SelectedProperty = await propertiesService!.GetImovel_ById(PropertyId);
            OriginalPropertyData = await propertiesService.GetImovel_ById(PropertyId); // TODO  should have used 'Clone/MemberWise'

            DeletePropertyCaption = SelectedProperty.Descricao;

            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                AddEditPropertyVisibility = true;
                EditCaption = L["EditMsg"] + " " + L["TituloImovel"];
                RecordMode = OpcoesRegisto.Gravar;
                units = await unitsService!.GetFracoes_Imovel(PropertyId);
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteCaption = L["DeleteMsg"] + " " + L["TituloImovel"];
                DeletePropertyVisibility = true;
            }
        }

        protected async Task UnitDoubleClickHandler(RecordDoubleClickEventArgs<FracaoVM> args)
        {
            UnitId = args.RowData.Id;
            modulo = Modules.Fracoes;
            SelectedUnit = await unitsService!.GetFracao_ById(UnitId);
            OriginalUnitData = await unitsService!.GetFracao_ById(UnitId);
            EditCaption = L["EditMsg"] + " " + L["TituloFracao"];
            RecordMode = OpcoesRegisto.Gravar;
            AddEditUnitVisibility = true;
        }

        /// <summary>
        ///  Command handler for editing/deleting property units
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task OnUnitCommandClicked(CommandClickEventArgs<FracaoVM> args)
        {
            UnitId = args.RowData.Id;
            modulo = Modules.Fracoes;
            SelectedUnit = await unitsService!.GetFracao_ById(UnitId);
            OriginalUnitData = await unitsService!.GetFracao_ById(UnitId);

            DeleteUnitCaption = $"[{SelectedUnit.Descricao}";

            if (args.CommandColumn.Type == CommandButtonType.Edit)
            {
                EditCaption = L["EditMsg"] + " " + L["TituloFracao"];
                RecordMode = OpcoesRegisto.Gravar;
                AddEditUnitVisibility = true;
            }

            if (args.CommandColumn.Type == CommandButtonType.Delete)
            {
                DeleteCaption = L["DeleteMsg"] + " " + L["TituloFracao"];
                DeleteUnitVisibility = true;
            }
        }



        /// <summary>
        /// Validate and update selected property in the grid (if changed)
        /// </summary>
        /// <returns></returns>
        public async Task SavePropertytData()
        {
            IsDirty = false;
            ErrorVisibility = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationsMessages = validatorService!.ValidatePropertyEntries(SelectedProperty!);

            if (ValidationsMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    //CheckIfPropertyData_Changed(); // 08/2022
                    //if (IsDirty) // registo alterado
                    //{
                    //    var updateOk = await propertiesService!.AtualizaImovel(SelectedProperty!.Id, SelectedProperty);
                    //}
                    ToastTitle = L["btnSalvar"] + " " + L["TituloImovel"];

                    var updateOk = await propertiesService!.AtualizaImovel(SelectedProperty!.Id, SelectedProperty);
                    if (updateOk)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["SuccessUpdate"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["MSG_ApiError"];
                        ToastIcon = "fas fa-exclamation";
                    }
                }

                else // Insert
                {
                    ToastTitle = L["NewMsg"] + " " + L["TituloImovel"];
                    if (string.IsNullOrEmpty(SelectedProperty.Foto))
                    {
                        ValidationsMessages = new List<string> { "Escolha foto da propriedade, p.f." };
                        ErrorVisibility = true;
                        return;

                    }
                    var insertOk = await propertiesService!.InsereImovel(SelectedProperty!);
                    if (insertOk)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["SuccessUpdate"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["FalhaCriacaoRegisto"];
                        ToastIcon = "fas fa-exclamation";
                    }
                }

                AddEditPropertyVisibility = false;

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

                properties = await GetAllProperties();
                await propertiesGridObj!.Refresh();
            }
            else
            {
                //AddEditPropertyVisibility = true;
                ErrorVisibility = true;
                await Task.Delay(100);
                await SpinnerObj!.HideAsync();
            }
        }

        /// <summary>
        /// Save selected unit data
        /// </summary>
        /// <returns></returns>
        public async Task SaveUnitData()
        {
            IsDirty = false;
            WarningMessage = string.Empty;
            WarningVisibility = false;

            ValidationsMessages = validatorService!.ValidateUnitEntries(SelectedUnit!);

            if (ValidationsMessages == null)
            {
                if (RecordMode == OpcoesRegisto.Gravar)
                {
                    ToastTitle = L["EditMsg"] + " " + L["TituloFracao"];

                    var unitUpdated = await unitsService!.AtualizaFracao(SelectedUnit!.Id, SelectedUnit);
                    if (unitUpdated)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["SuccessUpdate"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["FalhaGravacaoRegisto"];
                        ToastIcon = "fas fa-exclamation";
                    }
                }

                else // Insert
                {
                    ToastTitle = L["NewtMsg"] + " " + L["TituloFracao"];

                    var unitInserted = await unitsService!.InsereFracao(SelectedUnit!);
                    if (unitInserted)
                    {
                        ToastCss = "e-toast-success";
                        ToastMessage = L["SuccessUpdate"];
                        ToastIcon = "fas fa-check";
                    }
                    else
                    {
                        ToastCss = "e-toast-danger";
                        ToastMessage = L["FalhaCriacaoRegisto"];
                        ToastIcon = "fas fa-exclamation";
                    }

                    IsDirty = true;
                }

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();

                AddEditUnitVisibility = false;

                if (IsDirty)
                {
                    IsDirty = false;
                    properties = await GetAllProperties();
                    await propertiesGridObj!.Refresh();
                }
            }
            else
            {
                //AddEditUnitVisibility = false;
                await Task.Delay(100);
                await SpinnerObj!.HideAsync();
                WarningMessage = $"{ValidationsMessages[0]}";
                ErrorVisibility = true;
            }
        }


        private void CheckIfPropertyData_Changed()
        {
            var comparer = new ObjectsComparer.Comparer<ImovelVM>();
            IEnumerable<Difference> differences;
            comparer.Compare(SelectedProperty!, OriginalPropertyData!, out differences);
            IsDirty = differences.Any();
        }

        private void CheckIfUnitData_Changed()
        {
            var comparer = new ObjectsComparer.Comparer<FracaoVM>();
            IEnumerable<Difference> differences;
            comparer.Compare(SelectedUnit!, OriginalUnitData!, out differences);
            IsDirty = differences.Any();
        }

        /// <summary>
        ///  Prepare data to create unit
        /// </summary>
        /// <param name="args"></param>
        public void onAddUnit(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            RecordMode = OpcoesRegisto.Inserir;
            modulo = Modules.Fracoes;
            NewCaption = L["NewMsg"] + " " + L["TituloFracao"];
            SelectedUnit = new FracaoVM()
            {
                Ativa = true,
                Descricao = "",
                AreaBrutaPrivativa = 0,
                AreaBrutaDependente = 0,
                CasasBanho = 1,
                CozinhaEquipada = false,
                Varanda = true,
                Terraco = false,
                Garagem = false,
                Arrecadacao = true,
                GasCanalizado = true,
                LugarEstacionamento = false,
                Fotos = false,
                Notas = "",
                Tipologia = 1,
                ID_CertificadoEnergetico = 3,
                Matriz = "",
                LicencaHabitacao = "",
                DataEmissaoLicencaHabitacao = DateTime.UtcNow,
                Andar = "",
                Lado = "",
                AnoUltAvaliacao = DateTime.Today.Year.ToString(),
                ValorUltAvaliacao = 0,
                ValorRenda = 0,
                Id_TipoPropriedade = 1,
                Id_Imovel = PropertyId,
                Situacao = 2, // Livre (replaced) - not to be used!
                Conservacao = 1
            };

            AddEditUnitVisibility = true;
        }

        /// <summary>
        /// Prepare record for creation
        /// </summary>
        /// <param name="args"></param>
        public void onAddProperty(Microsoft.AspNetCore.Components.Web.MouseEventArgs args)
        {
            RecordMode = OpcoesRegisto.Inserir;
            modulo = Modules.Imoveis;
            NewCaption = L["NewMsg"] + " " + L["TituloImovel"];

            SelectedProperty = new ImovelVM()
            {
                AnoConstrucao = DateTime.UtcNow.Year.ToString(),
                CodPst = "",
                CodPstEx = "",
                ConcelhoImovel = "",
                Conservacao = 1,
                DataUltimaInspecaoGas = DateTime.UtcNow,
                Descricao = "",
                Elevador = false,
                FreguesiaImovel = "",
                Morada = "",
                Notas = "",
                Numero = ""
            };

            AddEditPropertyVisibility = true;
        }


        /// <summary>
        /// Close Add/Edit dialog
        /// </summary>
        protected void CloseEditDialog()
        {
            IsDirty = false;
            ErrorVisibility = false;

            switch (modulo)
            {
                case Modules.Imoveis:
                    var comparer_P = new ObjectsComparer.Comparer<ImovelVM>();
                    var currentData_P = SelectedProperty;
                    var originalData_P = OriginalPropertyData;
                    IEnumerable<Difference> differences_P;
                    var isEqual_P = comparer_P.Compare(currentData_P!, originalData_P!, out differences_P);
                    if (!isEqual_P)
                    {
                        IsDirty = true;
                    }
                    else
                    {
                        AddEditPropertyVisibility = false;

                    }

                    break;
                case Modules.Fracoes:

                    var comparer_U = new ObjectsComparer.Comparer<FracaoVM>();
                    var currentData_U = SelectedUnit;
                    var originalData_U = OriginalUnitData;
                    IEnumerable<Difference> differences_U;
                    var isEqual_U = comparer_U.Compare(currentData_U!, originalData_U!, out differences_U);
                    if (!isEqual_U)
                    {
                        IsDirty = true;
                    }
                    else
                    {
                        AddEditUnitVisibility = false;
                    }

                    break;
            }
        }

        protected void ContinueEdit()
        {
            IsDirty = false;

            switch (modulo)
            {
                case Modules.Imoveis:
                    AddEditPropertyVisibility = true;
                    break;
                case Modules.Fracoes:
                    AddEditUnitVisibility = true;
                    break;
            }
        }

        protected void IgnoreChangesAlert()
        {
            IsDirty = false;
            ErrorVisibility = false;
            switch (modulo)
            {
                case Modules.Imoveis:
                    AddEditPropertyVisibility = false;
                    break;
                case Modules.Fracoes:
                    AddEditUnitVisibility = false;
                    break;
            }
        }

        public async Task ConfirmDeleteYes()
        {
            WarningVisibility = false;
            WarningMessage = "";
            ToastTitle = L["DeleteMsg"] + " " + L["TituloImovel"];

            try
            {
                switch (modulo)
                {
                    case Modules.Imoveis:
                        DeletePropertyVisibility = false;
                        var resultOk = await propertiesService!.ApagaImovel(SelectedProperty!.Id);
                        if (resultOk)
                        {
                            properties = await GetAllProperties();
                            await propertiesGridObj!.Refresh();
                            ToastCss = "e-toast-success";
                            ToastMessage = L["SuccessUpdate"];
                            ToastIcon = "fas fa-check";

                        }
                        else
                        {
                            ToastCss = "e-toast-danger";
                            ToastMessage = "Não foi possível concluir operação. Imóvel com frações associadas!";
                            ToastIcon = "fas fa-exclamation";
                        }
                        break;
                    case Modules.Fracoes:
                        ToastTitle = "Apagar Fração";
                        try
                        {
                            await unitsService!.ApagaFracao(SelectedUnit!.Id);
                            DeleteUnitVisibility = false;
                            properties = await GetAllProperties();
                            await propertiesGridObj!.Refresh();
                            ToastCss = "e-toast-success";
                            ToastMessage = L["SuccessUpdate"];
                            ToastIcon = "fas fa-check";
                        }
                        catch (Exception ex)
                        {
                            ToastCss = "e-toast-danger";
                            ToastMessage = $"Não foi possível concluir operação - Erro: {ex.Message}";
                            ToastIcon = "fas fa-exclamation";
                        }
                        break;
                }

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();


            }
            catch (Exception exc)
            {
                ToastTitle = "Imóveis/Frações";
                ToastCss = "e-toast-danger";
                ToastMessage = $"Erro no módulo de Imóveis/Frações: {exc.Message}";
                ToastIcon = "fas fa-exclamation";

                StateHasChanged();
                await Task.Delay(100);
                await ToastObj!.ShowAsync();
            }
        }

        public void ConfirmDeleteNo()
        {
            switch (modulo)
            {
                case Modules.Imoveis:
                    DeletePropertyVisibility = false;
                    break;
                case Modules.Fracoes:
                    DeleteUnitVisibility = false;
                    break;
            }
        }

        public void CloseValidationErrorBox()
        {
            ErrorVisibility = false;

            switch (modulo)
            {
                case Modules.Imoveis:
                    AddEditPropertyVisibility = true;
                    break;
                case Modules.Fracoes:
                    AddEditUnitVisibility = true;
                    break;
            }
        }

        protected async Task HideToast()
        {
            await ToastObj!.HideAsync();
        }

        protected void HandleImagesAdded(List<NovaImagemFracao> unitImages)
        {
            UnitImages = unitImages;
            SelectedUnit!.Imagens = UnitImages;
        }

        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public void Dispose()
        {
            SpinnerObj?.Dispose();
            propertiesGridObj?.Dispose();
            unitsObj?.Dispose(); // grid
            ToastObj?.Dispose();
        }
    }
}
