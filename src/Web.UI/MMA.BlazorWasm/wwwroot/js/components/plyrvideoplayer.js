window.plyrInterop = {
    initializePlayer: function (playerId, inputId, wrapperId, allowedExtensions) {
        const player = new Plyr(`#${playerId}`, {
            captions: { active: true, update: true }
        });

        const video = document.getElementById(playerId);
        const subtitleInput = document.getElementById(inputId);
        const wrapper = document.getElementById(wrapperId);

        // Inject upload subtitle button
        function injectUploadSubtitleButton() {
            const captionsMenu = document.querySelector('[id^="plyr-settings-"][id$="-captions"] > [role="menu"]');
            if (!captionsMenu || captionsMenu.querySelector('.plyr-upload-sub')) return;

            const uploadBtn = document.createElement('button');
            uploadBtn.type = 'button';
            uploadBtn.className = 'plyr__control plyr-upload-sub btn btn-light border-0';
            uploadBtn.innerHTML = '<i class="bi bi-upload me-2"></i> Add Subtitle';

            uploadBtn.addEventListener('click', () => subtitleInput.click());
            captionsMenu.appendChild(uploadBtn);
        }

        // Inject loop toggle button
        function injectLoopToggleButton() {
            const settingsMenu = document.querySelector('[id^="plyr-settings-"] [role="menu"]');
            if (!settingsMenu || settingsMenu.querySelector('.plyr-toggle-loop')) return;

            const loopBtn = document.createElement('button');
            loopBtn.type = 'button';
            loopBtn.className = `plyr__control plyr-toggle-loop btn text-dark mt-1 border border-0`;
            loopBtn.setAttribute('aria-checked', video.loop);
            loopBtn.innerHTML = video.loop
                ? '<i class="bi bi-check-circle me-2 text-primary"></i> Loop'
                : '<i class="bi bi-arrow-repeat me-2"></i> Loop';

            loopBtn.addEventListener('click', () => {
                video.loop = !video.loop;
                loopBtn.setAttribute('aria-checked', video.loop);
                loopBtn.innerHTML = video.loop
                    ? '<i class="bi bi-check-circle me-2 text-primary"></i> Loop'
                    : '<i class="bi bi-arrow-repeat me-2"></i> Loop';
                loopBtn.className = `plyr__control plyr-toggle-loop btn ${video.loop ? 'btn-info' : ''} mt-1 text-dark border-0`;
            });

            settingsMenu.appendChild(loopBtn);
        }

        // Observe menu
        const observer = new MutationObserver(() => {
            injectUploadSubtitleButton();
            injectLoopToggleButton();
        });
        observer.observe(document.body, { childList: true, subtree: true });

        // Handle subtitle upload
        subtitleInput.addEventListener('change', function () {
            const file = this.files[0];
            if (!file || !allowedExtensions.some(ext => file.name.endsWith(ext))) {
                alert('Please upload a valid subtitle file.');
                return;
            }

            const oldTracks = video.querySelectorAll('track');
            oldTracks.forEach(t => t.remove());

            const track = document.createElement('track');
            track.kind = 'subtitles';
            track.label = 'Custom';
            track.srclang = 'en';
            track.default = true;
            track.dataset.dynamic = true;
            track.src = URL.createObjectURL(file);

            video.appendChild(track);
            player.toggleCaptions(true);
        });

        // Make resizable
        const handle = wrapper.querySelector('.resize-handle');
        if (!handle) return;

        let isResizing = false;
        handle.addEventListener('mousedown', function (e) {
            isResizing = true;
            const startX = e.clientX;
            const startY = e.clientY;
            const startWidth = wrapper.offsetWidth;
            const startHeight = wrapper.offsetHeight;

            function onMouseMove(e) {
                if (!isResizing) return;
                wrapper.style.width = `${startWidth + (e.clientX - startX)}px`;
                wrapper.style.height = `${startHeight + (e.clientY - startY)}px`;
            }

            function onMouseUp() {
                isResizing = false;
                window.removeEventListener('mousemove', onMouseMove);
                window.removeEventListener('mouseup', onMouseUp);
            }

            window.addEventListener('mousemove', onMouseMove);
            window.addEventListener('mouseup', onMouseUp);
        });
    }
};
