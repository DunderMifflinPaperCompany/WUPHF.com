// WUPHF.js - The Future of Communication
"use strict";

class WuphfApp {
    constructor() {
        this.connection = null;
        this.woofCount = 0;
        this.init();
    }

    async init() {
        await this.setupSignalR();
        this.setupEventListeners();
        this.loadRecentWuphfs();
        this.setupCharacterCounter();
        this.showWelcomeMessage();
    }

    async setupSignalR() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/wuphfhub")
            .build();

        // Handle incoming WUPHFs
        this.connection.on("ReceiveWuphf", (wuphf) => {
            this.displayWuphf(wuphf);
            this.playWoofSound();
        });

        // Handle notifications
        this.connection.on("ReceiveNotification", (user, message, channel) => {
            this.showNotification(user, message, channel);
        });

        // Handle printer output
        this.connection.on("PrintWuphf", (printOutput) => {
            this.showPrinterOutput(printOutput);
        });

        // Handle woof sound
        this.connection.on("PlayWoof", () => {
            this.playWoofSound();
        });

        // Handle likes and rewuphfs
        this.connection.on("WuphfLiked", (wuphfId) => {
            this.updateWuphfStats(wuphfId, 'like');
        });

        this.connection.on("WuphfRewuphfed", (wuphfId) => {
            this.updateWuphfStats(wuphfId, 'rewuphf');
        });

        try {
            await this.connection.start();
            console.log("SignalR Connected - WUPHF is LIVE! 🐕");
        } catch (err) {
            console.error(err);
        }
    }

    setupEventListeners() {
        // Send WUPHF button
        document.getElementById('sendWuphfBtn').addEventListener('click', () => {
            this.sendWuphf();
        });

        // Demo mode button
        document.getElementById('demoModeBtn').addEventListener('click', () => {
            this.activateDemoMode();
        });

        // Enter key in content area
        document.getElementById('wuphfContent').addEventListener('keypress', (e) => {
            if (e.key === 'Enter' && e.ctrlKey) {
                this.sendWuphf();
            }
        });
    }

    setupCharacterCounter() {
        const contentTextarea = document.getElementById('wuphfContent');
        const charCount = document.getElementById('charCount');

        contentTextarea.addEventListener('input', () => {
            const count = contentTextarea.value.length;
            charCount.textContent = count;
            
            if (count > 280) {
                charCount.style.color = 'red';
            } else if (count > 250) {
                charCount.style.color = 'orange';
            } else {
                charCount.style.color = 'green';
            }
        });
    }

    async sendWuphf() {
        const content = document.getElementById('wuphfContent').value.trim();
        const author = document.getElementById('authorName').value.trim() || 'Anonymous';
        const isUrgent = document.getElementById('urgentWuphf').checked;

        if (!content) {
            this.showAlert('Please enter a message!', 'warning');
            return;
        }

        if (content.length > 280) {
            this.showAlert('Message too long! Keep it under 280 characters.', 'danger');
            return;
        }

        // Get selected channels
        const channels = [];
        document.querySelectorAll('.channel-selector input[type="checkbox"]:checked').forEach(checkbox => {
            channels.push(checkbox.value);
        });

        if (channels.length === 0) {
            this.showAlert('Please select at least one notification channel!', 'warning');
            return;
        }

        const wuphfData = {
            content: content,
            author: author,
            channels: channels,
            isUrgent: isUrgent
        };

        try {
            const response = await fetch('/Wuphf/SendWuphf', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(wuphfData)
            });

            const result = await response.json();

            if (result.success) {
                document.getElementById('wuphfContent').value = '';
                document.getElementById('charCount').textContent = '0';
                this.showAlert('WUPHF sent successfully! 🚀', 'success');
                
                // Show notifications
                result.notifications.forEach((notification, index) => {
                    setTimeout(() => {
                        this.addNotificationToPanel(notification);
                    }, index * 500);
                });
            } else {
                this.showAlert('Failed to send WUPHF. Try again!', 'danger');
            }
        } catch (error) {
            console.error('Error sending WUPHF:', error);
            this.showAlert('Error sending WUPHF. Please try again!', 'danger');
        }
    }

    async activateDemoMode() {
        this.showAlert('🎉 DEMO MODE ACTIVATED! Brace yourself for the ULTIMATE WUPHF experience!', 'info');
        
        try {
            const response = await fetch('/Wuphf/DemoMode', {
                method: 'POST'
            });

            const result = await response.json();
            
            if (result.success) {
                // Show dramatic demo message
                setTimeout(() => {
                    this.showAlert('🐕 WOOF! This is what a WUPHF sounds like!', 'warning');
                }, 1000);
            }
        } catch (error) {
            console.error('Demo mode error:', error);
        }
    }

    async loadRecentWuphfs() {
        try {
            const response = await fetch('/Wuphf/GetRecentWuphfs');
            const wuphfs = await response.json();
            
            const feed = document.getElementById('wuphfFeed');
            feed.innerHTML = '';

            wuphfs.forEach(wuphf => {
                this.displayWuphf(wuphf, false);
            });
        } catch (error) {
            console.error('Error loading WUPHFs:', error);
        }
    }

    displayWuphf(wuphf, isNew = true) {
        const feed = document.getElementById('wuphfFeed');
        const wuphfElement = document.createElement('div');
        wuphfElement.className = `wuphf-item ${wuphf.isUrgent ? 'urgent' : ''}`;
        wuphfElement.setAttribute('data-wuphf-id', wuphf.id);

        const channelBadges = wuphf.notificationChannels.map(channel => 
            `<span class="channel-badge">${this.getChannelIcon(channel)} ${channel}</span>`
        ).join(' ');

        wuphfElement.innerHTML = `
            <div class="wuphf-author">
                ${wuphf.isUrgent ? '🚨 ' : ''}${wuphf.authorName}
                ${wuphf.isUrgent ? ' <span class="badge badge-danger">URGENT</span>' : ''}
            </div>
            <div class="wuphf-content">${this.formatContent(wuphf.content)}</div>
            <div class="wuphf-channels">${channelBadges}</div>
            <div class="wuphf-meta">
                <small class="text-muted">${this.formatDate(wuphf.createdAt)}</small>
                <div class="wuphf-actions">
                    <button class="btn btn-sm btn-outline-primary" onclick="wuphfApp.likeWuphf(${wuphf.id})">
                        👍 <span class="like-count">${wuphf.likes}</span>
                    </button>
                    <button class="btn btn-sm btn-outline-success" onclick="wuphfApp.rewuphfWuphf(${wuphf.id})">
                        🔄 <span class="rewuphf-count">${wuphf.rewuphfs}</span>
                    </button>
                    <button class="btn btn-sm btn-outline-secondary" onclick="wuphfApp.printWuphf(${wuphf.id})">
                        🖨️ Print
                    </button>
                </div>
            </div>
        `;

        if (isNew) {
            feed.insertBefore(wuphfElement, feed.firstChild);
            wuphfElement.style.animation = 'slideIn 0.5s ease-out';
        } else {
            feed.appendChild(wuphfElement);
        }
    }

    getChannelIcon(channel) {
        const icons = {
            'Email': '📧',
            'SMS': '📱',
            'Facebook': '📘',
            'Twitter': '🐦',
            'Printer': '🖨️',
            'HomePhone': '☎️',
            'Pager': '📟',
            'Fax': '📠'
        };
        return icons[channel] || '📢';
    }

    formatContent(content) {
        // Add basic emoji and hashtag support
        return content
            .replace(/#(\w+)/g, '<span style="color: #007bff; font-weight: bold;">#$1</span>')
            .replace(/WUPHF/gi, '<strong style="color: #ff6b6b;">WUPHF</strong>');
    }

    formatDate(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diffMinutes = Math.floor((now - date) / (1000 * 60));

        if (diffMinutes < 1) return 'Just now';
        if (diffMinutes < 60) return `${diffMinutes}m ago`;
        if (diffMinutes < 1440) return `${Math.floor(diffMinutes / 60)}h ago`;
        return date.toLocaleDateString();
    }

    async likeWuphf(wuphfId) {
        try {
            const response = await fetch('/Wuphf/LikeWuphf', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `id=${wuphfId}`
            });

            const result = await response.json();
            if (result.success) {
                this.updateWuphfStats(wuphfId, 'like');
            }
        } catch (error) {
            console.error('Error liking WUPHF:', error);
        }
    }

    async rewuphfWuphf(wuphfId) {
        try {
            const response = await fetch('/Wuphf/RewuphfWuphf', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `id=${wuphfId}`
            });

            const result = await response.json();
            if (result.success) {
                this.updateWuphfStats(wuphfId, 'rewuphf');
            }
        } catch (error) {
            console.error('Error rewuphfing WUPHF:', error);
        }
    }

    async printWuphf(wuphfId) {
        try {
            const response = await fetch('/Wuphf/PrintWuphf', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: `id=${wuphfId}`
            });

            const result = await response.json();
            if (result.success) {
                this.showPrinterOutput(result.printOutput);
                this.showAlert('WUPHF sent to printer! 🖨️', 'info');
            }
        } catch (error) {
            console.error('Error printing WUPHF:', error);
        }
    }

    updateWuphfStats(wuphfId, action) {
        const wuphfElement = document.querySelector(`[data-wuphf-id="${wuphfId}"]`);
        if (wuphfElement) {
            const countElement = wuphfElement.querySelector(`.${action}-count`);
            if (countElement) {
                const currentCount = parseInt(countElement.textContent);
                countElement.textContent = currentCount + 1;
                countElement.parentElement.style.animation = 'pulse 0.3s ease-out';
            }
        }
    }

    showNotification(user, message, channel) {
        this.addNotificationToPanel(`${user}: ${message}`);
    }

    addNotificationToPanel(message) {
        const panel = document.getElementById('notificationPanel');
        const notification = document.createElement('div');
        notification.className = 'notification-item';
        
        // Determine notification type based on content
        if (message.includes('Email')) notification.classList.add('notification-email');
        else if (message.includes('SMS') || message.includes('Text')) notification.classList.add('notification-sms');
        else if (message.includes('Facebook')) notification.classList.add('notification-facebook');
        else if (message.includes('Twitter')) notification.classList.add('notification-twitter');
        else if (message.includes('Print')) notification.classList.add('notification-printer');
        else if (message.includes('phone')) notification.classList.add('notification-homephone');

        notification.innerHTML = `
            <small class="text-muted">${new Date().toLocaleTimeString()}</small><br>
            ${message}
        `;

        panel.insertBefore(notification, panel.firstChild);

        // Remove old notifications (keep only last 10)
        while (panel.children.length > 10) {
            panel.removeChild(panel.lastChild);
        }

        panel.scrollTop = 0;
    }

    showPrinterOutput(output) {
        const printer = document.getElementById('printerOutput');
        const printElement = document.createElement('pre');
        printElement.style.marginBottom = '15px';
        printElement.style.borderBottom = '2px dashed #ccc';
        printElement.style.paddingBottom = '10px';
        printElement.textContent = output;

        printer.insertBefore(printElement, printer.firstChild);
        printer.scrollTop = 0;

        // Add printer sound effect simulation
        this.simulatePrinterSound();
    }

    playWoofSound() {
        this.woofCount++;
        
        // Try to play audio file
        const audio = document.getElementById('woofSound');
        if (audio) {
            audio.play().catch(() => {
                // Fallback: show visual woof
                this.showVisualWoof();
            });
        } else {
            this.showVisualWoof();
        }

        // Update woof counter
        console.log(`🐕 WOOF #${this.woofCount}!`);
    }

    showVisualWoof() {
        // Create visual woof effect
        const woof = document.createElement('div');
        woof.innerHTML = '🐕 WOOF!';
        woof.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            font-size: 3rem;
            background: rgba(255, 107, 107, 0.9);
            color: white;
            padding: 20px;
            border-radius: 15px;
            z-index: 9999;
            animation: woofEffect 1s ease-out forwards;
        `;

        // Add animation keyframes if not already added
        if (!document.getElementById('woofAnimation')) {
            const style = document.createElement('style');
            style.id = 'woofAnimation';
            style.textContent = `
                @keyframes woofEffect {
                    0% { opacity: 0; transform: translate(-50%, -50%) scale(0.5); }
                    50% { opacity: 1; transform: translate(-50%, -50%) scale(1.2); }
                    100% { opacity: 0; transform: translate(-50%, -50%) scale(0.8); }
                }
            `;
            document.head.appendChild(style);
        }

        document.body.appendChild(woof);

        setTimeout(() => {
            document.body.removeChild(woof);
        }, 1000);
    }

    simulatePrinterSound() {
        // Visual printer effect
        const printer = document.getElementById('printerOutput');
        printer.style.animation = 'none';
        setTimeout(() => {
            printer.style.animation = 'pulse 0.1s ease-in-out 5';
        }, 10);
    }

    showAlert(message, type) {
        // Create bootstrap alert
        const alertDiv = document.createElement('div');
        alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
        alertDiv.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        // Insert at top of page
        const container = document.querySelector('.wuphf-container');
        container.insertBefore(alertDiv, container.firstChild);

        // Auto dismiss after 5 seconds
        setTimeout(() => {
            if (alertDiv.parentNode) {
                alertDiv.remove();
            }
        }, 5000);
    }

    showWelcomeMessage() {
        setTimeout(() => {
            this.addNotificationToPanel('🎉 Welcome to WUPHF! The revolutionary communication platform is ready!');
            this.addNotificationToPanel('💡 Try the DEMO MODE button to experience the full WUPHF power!');
        }, 1000);
    }
}

// Initialize WUPHF App when page loads
let wuphfApp;
document.addEventListener('DOMContentLoaded', () => {
    wuphfApp = new WuphfApp();
});
