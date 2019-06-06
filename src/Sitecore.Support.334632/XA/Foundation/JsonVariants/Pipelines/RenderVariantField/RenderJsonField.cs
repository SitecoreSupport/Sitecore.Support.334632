namespace Sitecore.Support.XA.Foundation.JsonVariants.Pipelines.RenderVariantField
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using Sitecore.Pipelines;
    using Sitecore.XA.Foundation.JsonVariants.Fields;
    using Sitecore.XA.Foundation.JsonVariants.Renderers;
    using Sitecore.XA.Foundation.Variants.Abstractions.Fields;
    using Sitecore.XA.Foundation.Variants.Abstractions.Pipelines.RenderVariantField;
    using System.Web.UI;
    public class RenderJsonField : Sitecore.XA.Foundation.JsonVariants.Pipelines.RenderVariantField.RenderJsonField
    {
        public override void RenderField(RenderVariantFieldArgs args)
        {
            #region FIX for bug #334632
            // case, when we use "JSON Reference" field in the "JSON Content" rendering variants
            // and item, defined in the "Pass Through Field" field, is null;
            if (args.Item == null) return;

            #endregion

            VariantJsonField variantJsonField = args.VariantField as VariantJsonField;
            if (!string.IsNullOrWhiteSpace(variantJsonField?.FieldName))
            {
                string text = (!string.IsNullOrWhiteSpace(variantJsonField.Name)) ? variantJsonField.Name : variantJsonField.FieldName;
                BaseVariantField baseVariantField = ResolveJsonFallback(variantJsonField, args.Item, args.IsControlEditable);
                if (baseVariantField is VariantJsonField)
                {
                    VariantJsonField variantJsonField2 = baseVariantField as VariantJsonField;
                    Control control = CreateFieldRenderer(variantJsonField2, args.Item, variantJsonField2.FieldName, args.IsControlEditable, args.IsFromComposite);
                    string text4 = args.Result = (args.Result = ServiceLocator.ServiceProvider.GetService<IJsonRendererFactory>().Create(text.ToLower(), RenderControl(control), JsonValueType.String).ToJsonString());
                }
                else
                {
                    RenderVariantFieldArgs renderVariantFieldArgs = new RenderVariantFieldArgs(baseVariantField, args.Item)
                    {
                        IsControlEditable = args.IsControlEditable,
                        IsFromComposite = args.IsFromComposite,
                        RendererMode = args.RendererMode
                    };
                    CorePipeline.Run("renderVariantField", renderVariantFieldArgs);
                    args.Result = renderVariantFieldArgs.Result;
                }
            }
        }
    }
}