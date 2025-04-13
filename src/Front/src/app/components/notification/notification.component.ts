import { Component, computed, inject, OnDestroy, OnInit } from '@angular/core';
import { ToastModule } from 'primeng/toast';
import { MessageService } from 'primeng/api';
import { NotificationService } from '../../services/realtime/notification.service';
import { INotification } from '../../common/Interfaces/INotification';
import { Subject, takeUntil } from 'rxjs';
import { RouterLink } from '@angular/router';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [ToastModule, RouterLink, ButtonModule],
  templateUrl: './notification.component.html',
  styleUrl: './notification.component.css',
  providers: [MessageService, NotificationService],
})
export class NotificationComponent implements OnInit, OnDestroy {
  isConnected = computed(() => this._notifyService.connectionStatus());
  private _destroy$ = new Subject<void>();
  private _notifyService = inject(NotificationService);
  private _messageService = inject(MessageService);
  audio = new Audio('assets/notification.mp3');

  ngOnInit(): void {
    this.handleNotifyMessages();
  }
  ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  handleNotifyMessages() {
    this._notifyService.notification$
      .pipe(takeUntil(this._destroy$))
      .subscribe((value: INotification | null) => {
        if (value) {
          this.playSound();
          this._messageService.add({
            key: 'notify',
            severity: 'info',
            summary: value.Title,
            detail: value.Description,
            data: value.TargetUrl,
          });
        }
      });
  }
  playSound() {
    this.audio.play().catch((error: Error) => {
      console.error('Error playing sound:', error);
    });
  }
}
