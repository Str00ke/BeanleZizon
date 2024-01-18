public class ElementCollectible : ACollectible
{
    public Element element = Element.None;

    protected override void OnCollect()
    {
        Player.Instance.element = element;
        FindObjectOfType<BasicGeneratorMaster>().OnElementCollected(element);
    }
}