var inputCat = document.getElementById('categoryId');
var latLabel = document.getElementById('LatLabel');
var lngLabel = document.getElementById('LngLabel');
var descText = document.getElementById('descText');
var catLabel = document.getElementById('CatLabel');
var dateLabel = document.getElementById('DateLabel');
var severityLabel = document.getElementById('SeverityLabel');
var descLabel = document.getElementById('descLabel');
var hiddenLink = document.getElementById('hiddenLink');
var latVal = document.getElementById('latitude');
var lngVal = document.getElementById('longitude');

function setCategory(id) {
    switch (id) {
        case 0:
            inputCat.value = 1;
            break;
        case 1:
            inputCat.value = 2;
            break;
        case 2:
            inputCat.value = 3;
            break;
        case 3:
            inputCat.value = 4;
            break;
        case 4:
            inputCat.value = 5;
            break;
        case 5:
            inputCat.value = 6;
            break;
        case 6:
            inputCat.value = 7;
            break;
        case 7:
            inputCat.value = 8;
            break;
        case 8:
            inputCat.value = 9;
            break;
        default:
            inputCat.value = 0
    }
}
var imgRange = document.getElementById("imgRange");
imgRange.src = "/Content/img/Severities/5.png";

function changeImg() {
    var value = document.getElementById("rangeinput").value;
    var summaryCard = document.getElementById('summaryCard');
    switch (value) {
        case "1":
            imgRange.src = "/Content/img/Severities/1.png";
            summaryCard.style.backgroundColor = "#00FF00";
            
            break;
        case "2":
            imgRange.src = "/Content/img/Severities/2.png";
            summaryCard.style.backgroundColor = "#00FF00";
        case "3":
            imgRange.src = "/Content/img/Severities/3.png";
            summaryCard.style.backgroundColor = "#7FFF00";
            break;
        case "4":
            imgRange.src = "/Content/img/Severities/4.png";
            summaryCard.style.backgroundColor = "#7FFF00";
            break;
        case "5":
            imgRange.src = "/Content/img/Severities/5.png";
            summaryCard.style.backgroundColor = "#FFFF00";
            break;
        case "6":
            imgRange.src = "/Content/img/Severities/6.png";
            summaryCard.style.backgroundColor = "#FFFF00";
            break;
        case "7":
            imgRange.src = "/Content/img/Severities/7.png";
            summaryCard.style.backgroundColor = "#FFFF00";
            break;
        case "8":
            imgRange.src = "/Content/img/Severities/8.png";
            summaryCard.style.backgroundColor = "#FFFF00";
            break;
        case "9":
            imgRange.src = "/Content/img/Severities/9.png";
            summaryCard.style.backgroundColor = "#FF0000";
            break;
        case "10":
            imgRange.src = "/Content/img/Severities/10.png";
            summaryCard.style.backgroundColor = "#FF0000";
            break;
        default:
            imgRange.src = "/Content/img/Severities/10.png";
            break;
    }
}
var LocationTab = document.getElementById('LocationTab');
var SeverityTab = document.getElementById('SeverityTab');
var CategoryTab = document.getElementById('CategoryTab');
var ImageTab = document.getElementById('ImageTab');
var DescriptionTab = document.getElementById('DescriptionTab');
var SummaryTab = document.getElementById('SummaryTab');

CategoryTab.setAttribute("data-toggle", "");
CategoryTab.setAttribute("class", "disabled");
SeverityTab.setAttribute("data-toggle", "");
SeverityTab.setAttribute("class", "disabled");
DescriptionTab.setAttribute("data-toggle", "");
DescriptionTab.setAttribute("class", "disabled");
ImageTab.setAttribute("data-toggle", "");
ImageTab.setAttribute("class", "disabled");

SummaryTab.setAttribute("data-toggle", "");

function changeLocation() {
    LocationTab.setAttribute("data-toggle", "");
    LocationTab.setAttribute("class", "disabled");
    CategoryTab.setAttribute("data-toggle", "tab")
    CategoryTab.setAttribute("class", "");
    hiddenLink.onclick.apply(hiddenLink);
    CategoryTab.click.apply(CategoryTab);
}
function changeCategory() {
    CategoryTab.setAttribute("data-toggle", "");
    CategoryTab.setAttribute("class", "disabled");
    SeverityTab.setAttribute("data-toggle", "tab")
    SeverityTab.setAttribute("class", "");
    setSummary('category');
    SeverityTab.click.apply(SeverityTab);
}
function changeSeverity() {
    SeverityTab.setAttribute("data-toggle", "");
    SeverityTab.setAttribute("class", "disabled");
    ImageTab.setAttribute("data-toggle", "tab")
    ImageTab.setAttribute("class", "");
    setSummary('severity');
    ImageTab.click.apply(ImageTab);
    
}
function changeImage() {
    ImageTab.setAttribute("data-toggle", "");
    ImageTab.setAttribute("class", "disabled");
    DescriptionTab.setAttribute("data-toggle", "tab")
    DescriptionTab.setAttribute("class", "");
    
    DescriptionTab.click.apply(DescriptionTab);
}
function changeDescription() {
    DescriptionTab.setAttribute("data-toggle", "");
    DescriptionTab.setAttribute("class", "disabled");
    SummaryTab.setAttribute("data-toggle", "tab")
    SummaryTab.setAttribute("class", "");
    setSummary('description');
    SummaryTab.click.apply(SummaryTab);
}

function setSummary(text) {
    var sevValue = document.getElementById("rangeinput").value;
    switch (text) {
        case 'location':            
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1; //January is 0!
            var yyyy = today.getFullYear();

            if (dd < 10) {
                dd = '0' + dd
            }

            if (mm < 10) {
                mm = '0' + mm
            }

            today = mm + '/' + dd + '/' + yyyy;
            dateLabel.textContent = today;
            latLabel.textContent = latVal.value;
            lngLabel.textContent = lngVal.value;
            break;
        case 'category':
            catLabel.textContent = inputCat.value;
            break;
        case 'severity':
            severityLabel.textContent = sevValue;
            break;
        case 'description':
            descLabel.textContent = descText.value;
            break;
        default:
            break;
    }
}
//descText.addEventListener("keydown", function () {
//    changeDescription();
//    setSummary('description');
//}, false);