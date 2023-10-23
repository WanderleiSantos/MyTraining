using Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Models;

namespace Application.UseCases.SeriesPlannings.InsertSeriesPlanning.Commands;

public class InsertSeriesPlanningCommand
{
    public InsertSeriesPlanningCommand()
    {
        SeriesPlanningInputs = new List<SeriesPlanningInput>();
    }

    public List<SeriesPlanningInput> SeriesPlanningInputs { get; set; }
}