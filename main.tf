terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 4.0"
    }
  }
}

provider "aws" {
  region = "us-east-1"
}

resource "aws_s3_bucket" "foursixbucket" {
  bucket        = "foursixbucket"
  acl           = "private"
  force_destroy = true
}

data "archive_file" "lambda_archive" {
  type = "zip"
  source_dir  = "foursix-lambda/src/FourSix.Lambda.Authentication/bin/Release/net8.0/linux-x64/publish"
  output_path = "FourSix.Lambda.Authentication.zip"
}

resource "aws_s3_object" "lambda_bundle" {
  bucket = aws_s3_bucket.lambda_bucket.id

  key    = "FourSix.Lambda.Authentication.zip"
  source = data.archive_file.lambda_archive.output_path

  etag = filemd5(data.archive_file.lambda_archive.output_path)
}