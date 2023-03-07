$ && $.extend($.fn, {
    showLoading: function() {
        $('#loading').show();
        $('body').addClass('no-scroll');
    },
    hideLoading: function() {
        $('#loading').fadeOut('fast');
        $('body').removeClass('no-scroll');
    },
    pullToRefresh: function(selector, pullTitle, releaseTitle, _onRefresh) {
        $(".ptr--ptr").remove();
        PullToRefresh &&
            PullToRefresh.init({
                mainElement: "#main-container",
                triggerElement: selector,
                instructionsPullToRefresh: pullTitle,
                instructionsReleaseToRefresh: releaseTitle,
                onRefresh: function() {
                    if (_onRefresh) _onRefresh.call(this);
                    return location.reload();
                }
            });
    }
});
Object.defineProperty(String.prototype, 'capitalize', {
    value: function() {
        return this.charAt(0).toUpperCase() + this.slice(1);
    },
    enumerable: false,
});