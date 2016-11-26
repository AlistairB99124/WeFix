var inputCat = document.getElementById('categoryId');
var descText = document.getElementById('descText');
var catLabel = document.getElementById('CatLabel');
var dateLabel = document.getElementById('DateLabel');
var severityLabel = document.getElementById('SeverityLabel');
var descLabel = document.getElementById('descLabel');
var latVal = document.getElementById('latitude');
var lngVal = document.getElementById('longitude');
var addressLabel = document.getElementById('addressLabel');
var subCatLabel = document.getElementById('SubCatLabel');

var imgRange = document.getElementById("imgRange");
imgRange.src = "/Content/img/Severities/5.png";

function changeImg() {
    var value = document.getElementById("rangeinput").value;
    var summaryCard = document.getElementById('summaryCard');
    switch (value) {
        case "1":
            imgRange.src = "/Content/img/Severities/1.png";
            //summaryCard.style.backgroundColor = "#00FF00";
            
            break;
        case "2":
            imgRange.src = "/Content/img/Severities/2.png";
            //summaryCard.style.backgroundColor = "#00FF00";
        case "3":
            imgRange.src = "/Content/img/Severities/3.png";
            //summaryCard.style.backgroundColor = "#7FFF00";
            break;
        case "4":
            imgRange.src = "/Content/img/Severities/4.png";
            //summaryCard.style.backgroundColor = "#7FFF00";
            break;
        case "5":
            imgRange.src = "/Content/img/Severities/5.png";
            //summaryCard.style.backgroundColor = "#FFFF00";
            break;
        case "6":
            imgRange.src = "/Content/img/Severities/6.png";
            //summaryCard.style.backgroundColor = "#FFFF00";
            break;
        case "7":
            imgRange.src = "/Content/img/Severities/7.png";
            //summaryCard.style.backgroundColor = "#FFFF00";
            break;
        case "8":
            imgRange.src = "/Content/img/Severities/8.png";
            //summaryCard.style.backgroundColor = "#FFFF00";
            break;
        case "9":
            imgRange.src = "/Content/img/Severities/9.png";
            //summaryCard.style.backgroundColor = "#FF0000";
            break;
        case "10":
            imgRange.src = "/Content/img/Severities/10.png";
            //summaryCard.style.backgroundColor = "#FF0000";
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
    CategoryTab.click.apply(CategoryTab);
}
function changeCategory() {
    CategoryTab.setAttribute("data-toggle", "");
    CategoryTab.setAttribute("class", "disabled");
    SeverityTab.setAttribute("data-toggle", "tab")
    SeverityTab.setAttribute("class", "");
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

function BackToLocation() {    
    CategoryTab.setAttribute("data-toggle", "")
    CategoryTab.setAttribute("class", "hidden");
    LocationTab.setAttribute("data-toggle", "tab");
    LocationTab.setAttribute("class", "");    
    LocationTab.click.apply(LocationTab);
}
function BackToCategory() {
    SeverityTab.setAttribute("data-toggle", "")
    SeverityTab.setAttribute("class", "hidden");
    CategoryTab.setAttribute("data-toggle", "tab");
    CategoryTab.setAttribute("class", "");   
    CategoryTab.click.apply(CategoryTab);
}
function BackToSeverity() {    
    ImageTab.setAttribute("data-toggle", "")
    ImageTab.setAttribute("class", "hidden");
    SeverityTab.setAttribute("data-toggle", "tab");
    SeverityTab.setAttribute("class", "");
    SeverityTab.click.apply(SeverityTab);
}
function BackToImage() {    
    DescriptionTab.setAttribute("data-toggle", "")
    DescriptionTab.setAttribute("class", "hidden");
    ImageTab.setAttribute("data-toggle", "tab");
    ImageTab.setAttribute("class", "");
    ImageTab.click.apply(ImageTab);
}
function BackToDescription() {
   
    SummaryTab.setAttribute("data-toggle", "")
    SummaryTab.setAttribute("class", "hidden");
    DescriptionTab.setAttribute("data-toggle", "tab");
    DescriptionTab.setAttribute("class", "");
    DescriptionTab.click.apply(DescriptionTab);
}

function setSummary(text) {
    var sevValue = document.getElementById("rangeinput").value;
    
    switch (text) {
        case 'location':            
            var today = new Date();
            var dd = today.getDate();
            var mm = today.getMonth() + 1; //January is 0!
            var yyyy = today.getFullYear();
            var min = today.getMinutes();
            var sec = today.getSeconds();
            var hrs = today.getHours();

            if (dd < 10) {
                dd = '0' + dd
            }

            if (mm < 10) {
                mm = '0' + mm
            }
            if (hrs < 10) {
                hrs = '0' + hrs
            }
            if (min  < 10) {
                min = '0' + min
            }
            if (sec < 10) {
                sec = '0' + sec;
            }


            today = mm + '/' + dd + '/' + yyyy + ' ' + hrs + ':' + min + ':' + sec;
            dateLabel.textContent = today;
            var latlng = new google.maps.LatLng(latVal.value, lngVal.value);
            var geocoder = new google.maps.Geocoder;
            geocoder.geocode({ 'location': latlng }, function (results, status) {
                if (status === 'OK') {
                    if (results[0]) {
                        addressLabel.textContent = results[0].formatted_address;
                    }
                }
            });
            break;
        case 'severity':
            severityLabel.textContent = sevValue;
            break;
        case 'description':
            descLabel.textContent = descText.value;
            break;
        case 'SubCategory':
            var mySelect = document.getElementById('mySelect');
            var selectValue = mySelect.options[mySelect.selectedIndex].value;
            subCatLabel.textContent = selectValue;
            break;
        default:
            break;
    }
}
//descText.addEventListener("keydown", function () {
//    changeDescription();
//    setSummary('description');
//}, false);