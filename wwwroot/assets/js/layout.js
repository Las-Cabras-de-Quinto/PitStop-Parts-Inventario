// ===== LAYOUT PRINCIPAL JAVASCRIPT =====

document.addEventListener('DOMContentLoaded', function() {
    // Disable Bootstrap collapse functionality for our custom dropdowns
    const collapseElements = document.querySelectorAll('[data-bs-toggle="collapse"]');
    collapseElements.forEach(element => {
        element.removeAttribute('data-bs-toggle');
    });
    
    // Elements
    const hamburgerBtn = document.getElementById('hamburgerBtn');
    const sidebar = document.getElementById('sidebar');
    const sidebarOverlay = document.getElementById('sidebarOverlay');
    const dropdownToggles = document.querySelectorAll('.nav-section-title.dropdown-toggle');
    const navLinks = document.querySelectorAll('.nav-link');

    // Mobile sidebar toggle
    if (hamburgerBtn && sidebar && sidebarOverlay) {
        hamburgerBtn.addEventListener('click', function() {
            sidebar.classList.toggle('show');
            sidebarOverlay.classList.toggle('show');
            document.body.style.overflow = sidebar.classList.contains('show') ? 'hidden' : '';
        });

        // Close sidebar when clicking overlay
        sidebarOverlay.addEventListener('click', function() {
            sidebar.classList.remove('show');
            sidebarOverlay.classList.remove('show');
            document.body.style.overflow = '';
        });
    }

    // Handle dropdown toggles
    dropdownToggles.forEach(toggle => {
        toggle.addEventListener('click', function(e) {
            // Prevent Bootstrap from handling this
            e.preventDefault();
            e.stopPropagation();
            
            const target = this.getAttribute('data-bs-target');
            const dropdown = document.querySelector(target);
            
            if (dropdown) {
                const isExpanded = this.getAttribute('aria-expanded') === 'true';
                
                // Toggle the state
                if (isExpanded) {
                    this.setAttribute('aria-expanded', 'false');
                    dropdown.classList.remove('show');
                } else {
                    this.setAttribute('aria-expanded', 'true');
                    dropdown.classList.add('show');
                }
            }
        });
    });

    // Handle navigation link clicks
    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            // Remove active class from all links
            navLinks.forEach(l => l.classList.remove('active'));
            
            // Add active class to clicked link
            this.classList.add('active');

            // Close mobile sidebar if open
            if (window.innerWidth <= 768) {
                sidebar.classList.remove('show');
                sidebarOverlay.classList.remove('show');
                document.body.style.overflow = '';
            }

            // Store active link in localStorage
            localStorage.setItem('activeNavLink', this.href);
        });
    });

    // Restore active link on page load
    const activeLink = localStorage.getItem('activeNavLink');
    if (activeLink) {
        const link = document.querySelector(`a[href="${activeLink}"]`);
        if (link) {
            navLinks.forEach(l => l.classList.remove('active'));
            link.classList.add('active');
        }
    }

    // Handle window resize
    window.addEventListener('resize', function() {
        if (window.innerWidth > 768) {
            sidebar.classList.remove('show');
            sidebarOverlay.classList.remove('show');
            document.body.style.overflow = '';
        }
    });

    // Smooth scrolling for sidebar
    const sidebarNavLinks = sidebar.querySelectorAll('.nav-link');
    sidebarNavLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            // Add smooth transition effect
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = '';
            }, 150);
        });
    });

    // Auto-collapse inactive dropdowns on mobile
    function handleMobileDropdowns() {
        if (window.innerWidth <= 768) {
            const activeDropdown = document.querySelector('.nav-link.active')?.closest('.collapse');
            const allDropdowns = document.querySelectorAll('.collapse');
            
            allDropdowns.forEach(dropdown => {
                if (dropdown !== activeDropdown) {
                    dropdown.classList.remove('show');
                    const toggle = document.querySelector(`[data-bs-target="#${dropdown.id}"]`);
                    if (toggle) {
                        toggle.setAttribute('aria-expanded', 'false');
                    }
                }
            });
        }
    }

    // Call on window resize
    window.addEventListener('resize', handleMobileDropdowns);

    // Initialize tooltips if needed
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    if (typeof bootstrap !== 'undefined') {
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // Keyboard navigation
    document.addEventListener('keydown', function(e) {
        // ESC key closes mobile sidebar
        if (e.key === 'Escape' && sidebar.classList.contains('show')) {
            sidebar.classList.remove('show');
            sidebarOverlay.classList.remove('show');
            document.body.style.overflow = '';
        }
    });

    // Focus management for accessibility
    sidebar.addEventListener('keydown', function(e) {
        const focusableElements = this.querySelectorAll('a, button, [tabindex]:not([tabindex="-1"])');
        const firstElement = focusableElements[0];
        const lastElement = focusableElements[focusableElements.length - 1];

        if (e.key === 'Tab') {
            if (e.shiftKey) {
                if (document.activeElement === firstElement) {
                    lastElement.focus();
                    e.preventDefault();
                }
            } else {
                if (document.activeElement === lastElement) {
                    firstElement.focus();
                    e.preventDefault();
                }
            }
        }
    });

    // Update user info dynamically (if needed)
    function updateUserInfo(name, email) {
        const userNameElement = document.querySelector('.user-name');
        const userEmailElement = document.querySelector('.user-email');
        const avatarTextElement = document.querySelector('.avatar-text');

        if (userNameElement) userNameElement.textContent = name;
        if (userEmailElement) userEmailElement.textContent = email;
        if (avatarTextElement) {
            const initials = name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2);
            avatarTextElement.textContent = initials;
        }
    }

    // Expose updateUserInfo function globally if needed
    window.updateUserInfo = updateUserInfo;

    // Add loading states for navigation
    navLinks.forEach(link => {
        link.addEventListener('click', function() {
            if (this.href && !this.href.includes('#')) {
                // Add loading state
                const icon = this.querySelector('.nav-icon');
                const originalIcon = icon.className;
                icon.className = 'fas fa-spinner fa-spin nav-icon';
                
                // Restore icon after a short delay (or when page loads)
                setTimeout(() => {
                    icon.className = originalIcon;
                }, 1000);
            }
        });
    });

    // Set initial active state based on current page
    const currentPage = window.location.pathname.split('/').pop();
    if (currentPage) {
        const currentLink = document.querySelector(`a[href="${currentPage}"]`);
        if (currentLink) {
            navLinks.forEach(l => l.classList.remove('active'));
            currentLink.classList.add('active');
        }
    }

    console.log('Layout Principal JavaScript initialized successfully');
});
