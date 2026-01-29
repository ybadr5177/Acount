
document.addEventListener("DOMContentLoaded", () => {
    const slider = document.getElementById('categorySlider');
    const leftBtn = document.querySelector('.left-btn');
    const rightBtn = document.querySelector('.right-btn');
    console.log("✅ display-category.js loaded");
    const scrollAmount = 120; // مقدار التمرير لكل ضغطة

    leftBtn.addEventListener('click', () => {
        slider.scrollBy({ left: -scrollAmount, behavior: 'smooth' });
    });

    rightBtn.addEventListener('click', () => {
        slider.scrollBy({ left: scrollAmount, behavior: 'smooth' });
    });
    function updateButtonState() {
        leftBtn.disabled = slider.scrollLeft === 0;
        rightBtn.disabled = slider.scrollLeft + slider.clientWidth >= slider.scrollWidth;
    }

    updateButtonState();
    slider.addEventListener('scroll', updateButtonState);
});