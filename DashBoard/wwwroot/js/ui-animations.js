(function () {
	'use strict';
	// Reduce motion preference
	var prefersReduced = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)').matches;
	if (prefersReduced) return;

	// Hover lift for cards
	var liftables = document.querySelectorAll('.card, .hover-lift');
	liftables.forEach(function (el) {
		el.addEventListener('mouseenter', function () {
			el.classList.add('hover-lift');
		});
		el.addEventListener('mouseleave', function () {
			el.classList.remove('hover-lift');
		});
	});

	// Smooth sidebar show/hide (relies on existing menu toggle markup/classes)
	var layoutMenu = document.getElementById('layout-menu');
	var toggles = document.querySelectorAll('.layout-menu-toggle');
	toggles.forEach(function (t) {
		t.addEventListener('click', function () {
			if (!layoutMenu) return;
			layoutMenu.style.transition = 'transform 200ms cubic-bezier(.2,.6,.4,1), opacity 200ms';
		});
	});

	// Button press ripple (lightweight, no external deps)
	document.addEventListener('click', function (e) {
		var target = e.target.closest('.btn, .custom-button');
		if (!target) return;
		var rect = target.getBoundingClientRect();
		var ripple = document.createElement('span');
		ripple.style.position = 'absolute';
		ripple.style.left = (e.clientX - rect.left) + 'px';
		ripple.style.top = (e.clientY - rect.top) + 'px';
		ripple.style.width = ripple.style.height = Math.max(rect.width, rect.height) + 'px';
		ripple.style.background = 'currentColor';
		ripple.style.opacity = '0.15';
		ripple.style.borderRadius = '50%';
		ripple.style.transform = 'translate(-50%, -50%) scale(0)';
		ripple.style.pointerEvents = 'none';
		ripple.style.transition = 'transform 350ms ease, opacity 550ms ease';
		ripple.className = 'btn-ripple';
		target.style.position = target.style.position || 'relative';
		target.style.overflow = 'hidden';
		target.appendChild(ripple);
		requestAnimationFrame(function () {
			ripple.style.transform = 'translate(-50%, -50%) scale(1)';
			setTimeout(function () {
				ripple.style.opacity = '0';
				setTimeout(function () { ripple.remove(); }, 400);
			}, 200);
		});
	});

	// Interactive glow for tiles (mouse position -> CSS vars)
	document.addEventListener('pointermove', function (e) {
		var t = e.target.closest('.tile-card');
		if (!t) return;
		var r = t.getBoundingClientRect();
		var x = ((e.clientX - r.left) / r.width) * 100;
		var y = ((e.clientY - r.top) / r.height) * 100;
		t.style.setProperty('--mx', x + '%');
		t.style.setProperty('--my', y + '%');
	});

	// Theme toggle with persistence
	(function () {
		var body = document.body;
		var toggle = document.getElementById('themeToggle');
		var icon = toggle ? toggle.querySelector('.toggle-icon') : null;
		var STORAGE_KEY = 'dashboard-theme';
		var saved = localStorage.getItem(STORAGE_KEY);
		if (saved === 'dark') {
			body.classList.add('theme-dark');
		} else if (saved === 'light') {
			body.classList.remove('theme-dark');
		}
		function updateIcon() {
			if (!icon) return;
			var isDark = body.classList.contains('theme-dark');
			icon.textContent = isDark ? icon.getAttribute('data-dark') || '🌙' : icon.getAttribute('data-light') || '☀️';
		}
		updateIcon();
		if (toggle) {
			toggle.addEventListener('click', function () {
				var isDark = body.classList.toggle('theme-dark');
				localStorage.setItem(STORAGE_KEY, isDark ? 'dark' : 'light');
				updateIcon();
			});
		}
	})();

	// Sidebar toggle and persistence
	(function(){
		var body = document.body;
		var btn = document.getElementById('sidebarToggleBtn');
		var STORAGE_KEY = 'sidebar-collapsed';
		try {
			var saved = localStorage.getItem(STORAGE_KEY);
			if (saved === '1') body.classList.add('sidebar-collapsed');
		} catch(e) {}
		if (btn) {
			btn.addEventListener('click', function(){
				var collapsed = body.classList.toggle('sidebar-collapsed');
				try { localStorage.setItem(STORAGE_KEY, collapsed ? '1' : '0'); } catch(e) {}
			});
		}
	})();

	// Reveal-once animations using IntersectionObserver
	(function(){
		if (!('IntersectionObserver' in window)) return;
		var observer = new IntersectionObserver(function(entries){
			entries.forEach(function(entry){
				if (entry.isIntersecting){
					entry.target.classList.add('is-visible');
					observer.unobserve(entry.target);
				}
			});
		},{ threshold: 0.12 });
		document.querySelectorAll('.reveal-once').forEach(function(el){ observer.observe(el); });
	})();
})();
