using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRBookNavigation: MonoBehaviour
{
    public GameObject[] pages; // Pages of the book
    private int currentPageIndex = 0;

    public XRDirectInteractor pokeInteractor;  // The interactor (hand) that will poke the button
    public Collider nextPageButtonCollider;    // The collider of the next page button
    public Collider prevPageButtonCollider;    // The collider of the previous page button

    private void Start()
    {
        // Make sure the first page is visible
        UpdatePageVisibility();
    }

    private void OnEnable()
    {
        // Subscribe to the updated event using selectEntered
        pokeInteractor.selectEntered.AddListener(OnButtonPoked);  // Trigger on poke
    }

    private void OnDisable()
    {
        // Unregister the listener properly
        pokeInteractor.selectEntered.RemoveListener(OnButtonPoked);  // Unregister poke listener
    }

    private void OnButtonPoked(SelectEnterEventArgs args)
    {
        // Use interactorObject instead of the deprecated interactor
        if (nextPageButtonCollider.bounds.Contains(args.interactorObject.transform.position))
        {
            NextPage();
        }
        else if (prevPageButtonCollider.bounds.Contains(args.interactorObject.transform.position))
        {
            PreviousPage();
        }
    }

    // Show next page
    private void NextPage()
    {
        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            UpdatePageVisibility();
        }
    }

    // Show previous page
    private void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            UpdatePageVisibility();
        }
    }

    // Update visibility of pages
    private void UpdatePageVisibility()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(i == currentPageIndex);
        }
    }
}