import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { NgbModal, NgbModalOptions } from '@ng-bootstrap/ng-bootstrap';
import { ApiResponseModel } from './models/api-response-model';

@Injectable({
  providedIn: 'root',
})
export class SharedService {
  constructor(private toastr: ToastrService, private modalService: NgbModal) {}

  showNotification(
    apiResponse: ApiResponseModel<any>,
    backdrop: boolean = false
  ) {
    let isSuccess = false;

    if (apiResponse.statusCode >= 200 && apiResponse.statusCode < 300) {
      isSuccess = true;
    }

    if (apiResponse.showWithToastr) {
      if (isSuccess) {
        this.toastr.success(apiResponse.message, apiResponse.title);
      } else {
        this.toastr.error(apiResponse.message, apiResponse.title);
      }
    } else {
      const options: NgbModalOptions = {
        backdrop: backdrop ? 'static' : true,
      };
      const modalRef = this.modalService.open(Notification, options);
      modalRef.componentInstance.isSuccess = isSuccess;
      modalRef.componentInstance.title = apiResponse.title;
      modalRef.componentInstance.message = apiResponse.message;
      modalRef.componentInstance.isHtmlEnabled = apiResponse.isHtmlEnabled;
    }
  }
}
