function InitChart(chart, source) {
    if (chart.parentNode.parentNode.offsetHeight == 150) {
        window.setTimeout(function () {
            InitChart(chart, source);
        }, 100);
        return;
    }

    chart.removeAttribute('onload');
    chart.onload = undefined;

    chart.src = source;
}