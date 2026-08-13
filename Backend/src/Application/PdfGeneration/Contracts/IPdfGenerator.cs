namespace Application.PdfGeneration.Contracts;

public interface IPdfGenerator
{
    Task<byte[]> GenerateAsync<TModel>(
        PdfTemplate template,
        TModel model,
        CancellationToken cancellationToken = default);
}
