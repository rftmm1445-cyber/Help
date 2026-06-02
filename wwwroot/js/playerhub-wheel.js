(function () {
    const tasks = [
        { title: "خمن القارة فقط", note: "+30 نقطة عند النجاح" },
        { title: "خمن الدولة", note: "+60 نقطة عند النجاح" },
        { title: "خمن المدينة", note: "+100 نقطة عند النجاح" },
        { title: "خمن خلال 30 ثانية", note: "+80 نقطة إذا نجحت" },
        { title: "جولة بدون تلميحات", note: "+90 نقطة إذا نجحت" },
        { title: "خمن نصف الكرة (شمالي/جنوبي)", note: "+40 نقطة عند النجاح" },
        { title: "خمن نوع المنطقة (مدينة/ريف/ساحل)", note: "+50 نقطة عند النجاح" },
        { title: "حقق دقة عالية", note: "اقترب قدر الإمكان لتحصل على مكافأة" }
    ];

    const wheel = document.getElementById("dailyWheel");
    const spinBtn = document.getElementById("spinBtn");
    const overlay = document.getElementById("taskOverlay");
    const taskTitle = document.getElementById("taskTitle");
    const taskNote = document.getElementById("taskNote");
    const closeTask = document.getElementById("closeTask");

    const segmentCount = tasks.length;
    const segmentAngle = 360 / segmentCount;
    const todayKey = "dailyTask_" + new Date().toISOString().slice(0, 10);

    let isSpinning = false;
    let currentRotation = 0;

    function showTask(index) {
        taskTitle.textContent = tasks[index].title;
        taskNote.textContent = tasks[index].note;
        overlay.style.display = "flex";

        confetti({
            particleCount: 120,
            spread: 70,
            origin: { y: 0.6 }
        });
    }

    function spinWheel(index) {
        isSpinning = true;
        const extraSpins = 5;
        const finalRotation = 360 * extraSpins + index * segmentAngle + segmentAngle / 2;
        currentRotation = (currentRotation + finalRotation) % 360;
        wheel.style.transition = "transform 4s cubic-bezier(.15,.85,.15,1)";
        wheel.style.transform = `rotate(${currentRotation}deg)`;
        setTimeout(() => {
            showTask(index);
            isSpinning = false;
        }, 4200);
    }

    spinBtn.addEventListener("click", () => {
        if (isSpinning) return;
        const storedIndex = localStorage.getItem(todayKey);
        if (storedIndex !== null) {
            showTask(parseInt(storedIndex));
            return;
        }
        const randIndex = Math.floor(Math.random() * tasks.length);
        localStorage.setItem(todayKey, randIndex);
        spinWheel(randIndex);
    });

    closeTask.addEventListener("click", () => {
        overlay.style.display = "none";
    });

    const storedIndex = localStorage.getItem(todayKey);
    if (storedIndex !== null) {
        showTask(parseInt(storedIndex));
    }
})();
