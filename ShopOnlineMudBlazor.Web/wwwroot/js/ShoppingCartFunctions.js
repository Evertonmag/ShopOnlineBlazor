function MakeUpdateQtyButtonVisible(id, visible)
{
    const updateQTyButton = document.querySelector("button[data-itemId='" + id + "']")

    if (visible == true) {
        updateQTyButton.style.display = "inline-block";
    }
    else {
        updateQTyButton.style.display = "none";
    }
}