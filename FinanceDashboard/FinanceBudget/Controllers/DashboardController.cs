using Microsoft.AspNetCore.Mvc;
using FinanceBudget.Models;
using FinanceBudget.Services;

namespace FinanceBudget.Controllers;

public class DashboardController : Controller
{
    private readonly IUnitBudgetService _unitBudgetService;
    private readonly IAFEDataService _afeDataService;
    private readonly IJiraCostingService _jiraCostingService;
    private readonly IPlanfulService _planfulService;
    private readonly IAFEBudgetService _afeBudgetService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IUnitBudgetService unitBudgetService,
        IAFEDataService afeDataService,
        IJiraCostingService jiraCostingService,
        IPlanfulService planfulService,
        IAFEBudgetService afeBudgetService,
        ILogger<DashboardController> logger)
    {
        _unitBudgetService = unitBudgetService;
        _afeDataService = afeDataService;
        _jiraCostingService = jiraCostingService;
        _planfulService = planfulService;
        _afeBudgetService = afeBudgetService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? unitCode = null, string? natureId = null)
    {
        try
        {
            // Always fetch dropdown data
            var allUnits = await _unitBudgetService.GetAllUnitsAsync();

            // Initialize view model with dropdown data
            var viewModel = new DashboardViewModel
            {
                AvailableUnits = allUnits
            };

            // Only fetch data if a unit is selected
            if (!string.IsNullOrEmpty(unitCode))
            {
                // Fetch natures for selected unit
                var availableNatures = await _unitBudgetService.GetNaturesByUnitAsync(unitCode);
                viewModel.AvailableNatures = availableNatures;

                // Fetch data from all sources in parallel
                var unitBudgetTask = _unitBudgetService.GetUnitBudgetDataAsync(unitCode, natureId);
                var afeDataTask = _afeDataService.GetAFEDataAsync(unitCode);
                var jiraCostingTask = _jiraCostingService.GetJiraCostingDataAsync(unitCode);

                // New: Fetch Planful and AFE Budget data
                var planfulSummaryTask = _planfulService.GetPlanfulSummaryAsync(nature: natureId);
                var planfulDetailTask = _planfulService.GetPlanfulDetailAsync(unitCode, natureId);
                var planfulIntegrationTask = _planfulService.GetPlanfulIntegrationSummaryAsync(unitCode, natureId);
                var afeBudgetTask = _afeBudgetService.GetAFEBudgetsAsync(unit: unitCode);
                var afeMappingsTask = _afeBudgetService.GetAFEUnitNatureMappingsAsync(unit: unitCode);
                var businessVerticalsTask = _afeBudgetService.GetBusinessVerticalsAsync();
                var projectMappingsTask = _afeBudgetService.GetProjectVerticalMappingsAsync();

                await Task.WhenAll(unitBudgetTask, afeDataTask, jiraCostingTask,
                    planfulSummaryTask, planfulDetailTask, planfulIntegrationTask,
                    afeBudgetTask, afeMappingsTask, businessVerticalsTask, projectMappingsTask);

                var unitBudgets = await unitBudgetTask;
                var afeData = await afeDataTask;
                var jiraCostings = await jiraCostingTask;
                var planfulSummaries = await planfulSummaryTask;
                var planfulDetails = await planfulDetailTask;
                var planfulIntegrations = await planfulIntegrationTask;
                var afeBudgets = await afeBudgetTask;
                var afeMappings = await afeMappingsTask;
                var businessVerticals = await businessVerticalsTask;
                var projectMappings = await projectMappingsTask;

                // Calculate Credit (positive amounts) and Debit (negative amounts)
                var totalCredit = unitBudgets.Where(u => u.Amount > 0).Sum(u => u.Amount);
                var totalDebit = Math.Abs(unitBudgets.Where(u => u.Amount < 0).Sum(u => u.Amount));

                // Populate view model with data
                viewModel.UnitBudgets = unitBudgets;
                viewModel.AFEDataList = afeData;
                viewModel.JiraCostings = jiraCostings;
                viewModel.PlanfulSummaries = planfulSummaries;
                viewModel.PlanfulDetails = planfulDetails;
                viewModel.PlanfulIntegrationSummaries = planfulIntegrations;
                viewModel.AFEBudgets = afeBudgets;
                viewModel.AFEUnitNatureMappings = afeMappings;
                viewModel.BusinessVerticals = businessVerticals;
                viewModel.ProjectVerticalMappings = projectMappings;

                viewModel.TotalBudgetAmount = unitBudgets.Sum(u => u.Amount);
                viewModel.TotalBudgetCredit = totalCredit;
                viewModel.TotalBudgetDebit = totalDebit;
                viewModel.TotalAFEApproved = afeData.Sum(a => a.ApprovedAmount);
                viewModel.TotalAFESpent = afeData.Sum(a => a.SpentAmount);
                viewModel.TotalJiraEstimated = jiraCostings.Sum(j => j.EstimatedCost);
                viewModel.TotalJiraActual = jiraCostings.Sum(j => j.ActualCost);

                // New: Planful and AFE Budget totals
                viewModel.TotalPlanfulBudget = planfulIntegrations.Sum(p => p.TotalBudget);
                viewModel.TotalPlanfulActual = planfulIntegrations.Sum(p => p.TotalActual);
                viewModel.TotalPlanfulForecast = planfulIntegrations.Sum(p => p.TotalForecast);
                viewModel.TotalAFEBudgetApproved = afeBudgets.Sum(a => a.ApprovedAmount);
                viewModel.TotalAFEBudgetCommitted = afeBudgets.Sum(a => a.CommittedAmount);
                viewModel.TotalAFEBudgetForecast = afeBudgets.Sum(a => a.Forecast);

                // Build unit summaries
                var units = unitBudgets.Select(u => u.Unit).Distinct();
                foreach (var unit in units)
                {
                    var unitBudgetData = unitBudgets.Where(u => u.Unit == unit).ToList();
                    var unitAFEData = afeData.Where(a => a.Unit == unit).ToList();
                    var unitJiraData = jiraCostings.Where(j => j.Unit == unit).ToList();
                    var unitPlanfulData = planfulIntegrations.Where(p => p.Unit == unit).ToList();
                    var unitAFEBudgetData = afeBudgets.Where(a => a.Unit == unit).ToList();

                    viewModel.UnitSummaries[unit] = new UnitSummary
                    {
                        Unit = unit,
                        UnitDescription = unitBudgetData.FirstOrDefault()?.UnitDescription ?? "",
                        BudgetAmount = unitBudgetData.Sum(u => u.Amount),
                        AFEAmount = unitAFEData.Sum(a => a.ApprovedAmount),
                        JiraAmount = unitJiraData.Sum(j => j.EstimatedCost),
                        PlanfulBudget = unitPlanfulData.Sum(p => p.TotalBudget),
                        PlanfulActual = unitPlanfulData.Sum(p => p.TotalActual),
                        AFEBudgetApproved = unitAFEBudgetData.Sum(a => a.ApprovedAmount),
                        AFEBudgetCommitted = unitAFEBudgetData.Sum(a => a.CommittedAmount),
                        TotalAmount = unitBudgetData.Sum(u => u.Amount) +
                                      unitAFEData.Sum(a => a.ApprovedAmount) +
                                      unitJiraData.Sum(j => j.EstimatedCost) +
                                      unitPlanfulData.Sum(p => p.TotalActual) +
                                      unitAFEBudgetData.Sum(a => a.ApprovedAmount),
                        TransactionCount = unitBudgetData.Count,
                        AFECount = unitAFEData.Count,
                        JiraIssueCount = unitJiraData.Count
                    };
                }

                // Build business vertical summaries
                foreach (var vertical in businessVerticals)
                {
                    var verticalProjects = projectMappings.Where(m => m.BusinessVerticalCode == vertical.VerticalCode).ToList();
                    var verticalAFEBudgets = afeBudgets.Where(a =>
                        verticalProjects.Any(p => p.AFENumber == a.AFENumber || p.Project == a.Project)).ToList();
                    var verticalPlanful = planfulIntegrations.Where(p => p.BusinessVertical == vertical.VerticalCode).ToList();

                    viewModel.VerticalSummaries[vertical.VerticalCode] = new VerticalSummary
                    {
                        VerticalCode = vertical.VerticalCode,
                        VerticalName = vertical.VerticalName,
                        Description = vertical.Description,
                        TotalBudget = verticalAFEBudgets.Sum(a => a.ApprovedAmount) + verticalPlanful.Sum(p => p.TotalBudget),
                        TotalActual = verticalAFEBudgets.Sum(a => a.CommittedAmount) + verticalPlanful.Sum(p => p.TotalActual),
                        TotalForecast = verticalAFEBudgets.Sum(a => a.Forecast) + verticalPlanful.Sum(p => p.TotalForecast),
                        TotalVariance = verticalPlanful.Sum(p => p.TotalVariance),
                        AS400Budget = unitBudgets.Sum(u => u.Amount), // This would need proper mapping
                        PlanfulBudget = verticalPlanful.Sum(p => p.TotalBudget),
                        AFEBudget = verticalAFEBudgets.Sum(a => a.ApprovedAmount),
                        JiraBudget = jiraCostings.Sum(j => j.EstimatedCost), // This would need proper mapping
                        ProjectCount = verticalProjects.Select(p => p.Project).Distinct().Count(),
                        UnitCount = verticalAFEBudgets.Select(a => a.Unit).Distinct().Count(),
                        AFECount = verticalAFEBudgets.Count
                    };
                }
            }

            ViewBag.SelectedUnit = unitCode;
            ViewBag.SelectedNature = natureId;
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard");
            TempData["Error"] = "An error occurred while loading the dashboard. Please try again.";

            // Return empty view model with dropdown data
            var emptyViewModel = new DashboardViewModel();
            try
            {
                emptyViewModel.AvailableUnits = await _unitBudgetService.GetAllUnitsAsync();
            }
            catch { }

            return View(emptyViewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetNaturesByUnit(string unitCode)
    {
        try
        {
            var natures = await _unitBudgetService.GetNaturesByUnitAsync(unitCode);
            return Json(natures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching natures for unit {UnitCode}", unitCode);
            return BadRequest("Error fetching natures");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUnitDetails(string unitCode)
    {
        try
        {
            var unitBudgets = await _unitBudgetService.GetUnitBudgetDataAsync(unitCode);
            return Json(unitBudgets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching unit details");
            return BadRequest("Error fetching unit details");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAFEDetails(string unitCode)
    {
        try
        {
            var afeData = await _afeDataService.GetAFEDataAsync(unitCode);
            return Json(afeData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching AFE details");
            return BadRequest("Error fetching AFE details");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetJiraDetails(string unitCode)
    {
        try
        {
            var jiraData = await _jiraCostingService.GetJiraCostingDataAsync(unitCode);
            return Json(jiraData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching JIRA details");
            return BadRequest("Error fetching JIRA details");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ExportData(string? unitCode = null, string format = "json")
    {
        try
        {
            var unitBudgets = await _unitBudgetService.GetUnitBudgetDataAsync(unitCode);
            var afeData = await _afeDataService.GetAFEDataAsync(unitCode);
            var jiraCostings = await _jiraCostingService.GetJiraCostingDataAsync(unitCode);
            var planfulDetails = await _planfulService.GetPlanfulDetailAsync(unitCode);
            var afeBudgets = await _afeBudgetService.GetAFEBudgetsAsync(unit: unitCode);

            var exportData = new
            {
                UnitBudgets = unitBudgets,
                AFEData = afeData,
                JiraCostings = jiraCostings,
                PlanfulDetails = planfulDetails,
                AFEBudgets = afeBudgets,
                ExportDate = DateTime.Now
            };

            if (format.ToLower() == "json")
            {
                return Json(exportData);
            }

            return BadRequest("Unsupported export format");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting data");
            return BadRequest("Error exporting data");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPlanfulSummary(int? year = null, int? month = null, string? nature = null)
    {
        try
        {
            var planfulSummary = await _planfulService.GetPlanfulSummaryAsync(year, month, nature);
            return Json(planfulSummary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Planful summary");
            return BadRequest("Error fetching Planful summary");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPlanfulDetails(string? unitCode = null, string? natureId = null, int? year = null, int? month = null)
    {
        try
        {
            var planfulDetails = await _planfulService.GetPlanfulDetailAsync(unitCode, natureId, year, month);
            return Json(planfulDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching Planful details");
            return BadRequest("Error fetching Planful details");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAFEBudgets(string? project = null, string? afeNumber = null, string? unit = null)
    {
        try
        {
            var afeBudgets = await _afeBudgetService.GetAFEBudgetsAsync(project, afeNumber, unit);
            return Json(afeBudgets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching AFE budgets");
            return BadRequest("Error fetching AFE budgets");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetBusinessVerticals()
    {
        try
        {
            var verticals = await _afeBudgetService.GetBusinessVerticalsAsync();
            return Json(verticals);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching business verticals");
            return BadRequest("Error fetching business verticals");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ImportAFEBudgets([FromBody] List<AFEBudget> budgets)
    {
        try
        {
            var count = await _afeBudgetService.ImportAFEBudgetsAsync(budgets);
            return Json(new { success = true, count = count, message = $"Imported {count} AFE budgets successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing AFE budgets");
            return BadRequest(new { success = false, message = "Error importing AFE budgets" });
        }
    }

    [HttpPost]
    public async Task<IActionResult> LinkAFEToUnitNature([FromBody] AFEUnitNatureMapping mapping)
    {
        try
        {
            var success = await _afeBudgetService.LinkAFEToUnitNatureAsync(mapping);
            return Json(new { success = success, message = success ? "Linked successfully" : "Failed to link" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error linking AFE to unit nature");
            return BadRequest(new { success = false, message = "Error linking AFE to unit nature" });
        }
    }
}

