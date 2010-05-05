
function contextHelp(chContainer) {
    var hText = chContainer.getElementsByTagName("span").item(0);
    var hButton = chContainer.getElementsByTagName("a").item(0);
    if (hText.style.visibility == "hidden") {
        hText.style.visibility = "visible";
        hButton.innerText = hButton.innerText.replace(/Expand/i, 'Collapse');
    } else {
        hText.style.visibility = "hidden";
        hButton.innerText = hButton.innerText.replace(/Collapse/i, 'Expand');
    }
}

function expcoll(oButton) {
    var hAction;
    var chContainers = document.getElementsByTagName("p");
    if (oButton.innerText.search(/Collapse/i) >= 0) {
        hAction = false;
        oButton.innerText = oButton.innerText.replace(/Collapse/i, 'Expand');
    } else {
        hAction = true;
        oButton.innerText = oButton.innerText.replace(/Expand/i, 'Collapse');
    }
    for (var i = 0; i < chContainers.length; i++) {
        if (chContainers(i).className == "chelp") {
            if (hAction == false && chContainers(i).getElementsByTagName("span").item(0).style.visibility != "hidden") {
                contextHelp(chContainers(i));
            } else {
                if (hAction == true && chContainers(i).getElementsByTagName("span").item(0).style.visibility == "hidden") {
                    contextHelp(chContainers(i));
                }
            }
        }
    }
}
