// Marker class for shared resource lookup. Lives in the ROOT namespace on purpose:
// with ResourcesPath = "Resources", the localizer maps Lumora.Web.SharedResources
// to Resources/SharedResources.{culture}.resx. If this class sat in
// Lumora.Web.Resources, lookup would expect Resources/Resources/... and silently
// fall back to returning raw key names.
namespace Lumora.Web;

public class SharedResources { }
