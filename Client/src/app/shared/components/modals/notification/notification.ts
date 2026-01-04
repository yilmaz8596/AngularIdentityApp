import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-notification',
  imports: [],
  templateUrl: './notification.html',
  styleUrl: './notification.css',
})
export class Notification {
  @Input() isSuccess!: boolean;
  @Input() title!: string;
  @Input() message!: string;
  @Input() isHtmlEnabled: boolean = false;

  constructor(public activeModal: NgbActiveModal) {}
}
